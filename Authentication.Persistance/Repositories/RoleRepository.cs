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

            foreach (var policy in validPolicies) {
                _context.RolePolicies.Add(new RolePolicy(role.Id, policy.Id));
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
