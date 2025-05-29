using Microsoft.EntityFrameworkCore;

namespace Authentication.Persistance.Repositories {
    public class RoleRepository : IRoleRepository {
        private readonly AuthenticationDatabaseContext _context;

        public RoleRepository(AuthenticationDatabaseContext context) {
            _context = context;
        }

        public async Task CreateAsync(Role role) {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        //TODO: Improve error handling by adding bool back
        //TODO: On delete role, also remove the policies attached
        public async Task DeleteAsync(Guid id) {
            var role = await _context.Roles.FindAsync(id);

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Role>> GetAllAsync() {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetByIdAsync(Guid id) {
            return await _context.Roles.FindAsync(id);
        }

        //TODO: Improve error handling by adding bool back
        public async Task UpdateAsync(Role role) {
            var existingRole = await _context.Roles.FindAsync(role.Id);

            var createdAt = existingRole.CreatedAt;
            _context.Entry(existingRole).CurrentValues.SetValues(role);
            existingRole.CreatedAt = createdAt;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AssignPoliciesToRoleAsync(Guid roleId, List<Guid> policyIds) {
            var role = await _context.Roles
                .Include(r => r.Policies)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
                return false;

            var validPolicies = await _context.Policies
                .Where(p => policyIds.Contains(p.Id))
                .ToListAsync();

            if (validPolicies.Count != policyIds.Count)
                return false;

            var existingMappings = await _context.RolePolicies
                .Where(rp => rp.RoleId == roleId && policyIds.Contains(rp.PolicyId))
                .Select(rp => rp.PolicyId)
                .ToListAsync();

            var newPolicyIds = policyIds.Except(existingMappings);

            foreach (var policyId in newPolicyIds) {
                _context.RolePolicies.Add(new RolePolicy(roleId, policyId));
            }

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<List<Policy>> GetAllInheritedPoliciesAsync(Guid roleId) {
            var allRoleIds = await GetAllDescendantRoleIdsAsync(roleId);
            allRoleIds.Add(roleId); // include self

            var policies = await _context.RolePolicies
                .Where(rp => allRoleIds.Contains(rp.RoleId))
                .Include(rp => rp.Policy)
                .Select(rp => rp.Policy)
                .Distinct()
                .ToListAsync();

            return policies;
        }

        public async Task<List<Guid>> GetAllDescendantRoleIdsAsync(Guid rootRoleId) {
            var visited = new HashSet<Guid>();
            var toVisit = new Queue<Guid>();
            toVisit.Enqueue(rootRoleId);

            while (toVisit.Any()) {
                var current = toVisit.Dequeue();
                if (!visited.Add(current)) continue;

                var children = await _context.RoleInheritances
                    .Where(r => r.ParentRoleId == current)
                    .Select(r => r.ChildRoleId)
                    .ToListAsync();

                foreach (var child in children) {
                    if (!visited.Contains(child)) {
                        toVisit.Enqueue(child);
                    }
                }
            }

            visited.Remove(rootRoleId);
            return visited.ToList();
        }

        public async Task<bool> CreateRoleInheritanceAsync(Guid parentRoleId, Guid childRoleId) {
            if (parentRoleId == childRoleId)
                return false;

            // Check for circular inheritance: child should NOT already be a parent of the given parent
            var allDescendants = await GetAllDescendantRoleIdsAsync(childRoleId);

            if (allDescendants.Contains(parentRoleId))
                return false;

            var alreadyExists = await _context.RoleInheritances
                .AnyAsync(ri => ri.ParentRoleId == parentRoleId && ri.ChildRoleId == childRoleId);

            if (alreadyExists)
                return true;

            _context.RoleInheritances.Add(new RoleInheritance(parentRoleId, childRoleId));
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Policy>> GetRolePoliciesAsync(Guid roleId) {
            return await _context.RolePolicies
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Policy)
                .Select(rp => rp.Policy)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetChildRoleIdsAsync(Guid parentRoleId) {
            return await _context.RoleInheritances
                .Where(ri => ri.ParentRoleId == parentRoleId)
                .Select(ri => ri.ChildRoleId)
                .ToListAsync();
        }

        public async Task<List<Policy>> GetPoliciesForRolesAsync(List<Guid> roleIds) {
            return await _context.RolePolicies
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Include(rp => rp.Policy)
                .Select(rp => rp.Policy)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> RemovePolicyFromRoleAsync(Guid roleId, Guid policyId) {
            var existing = await _context.RolePolicies
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PolicyId == policyId);

            if (existing == null)
                return false;

            _context.RolePolicies.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
