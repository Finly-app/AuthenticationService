using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Persistance.Repositories {
    public class UserRepository : IUserRepository {
        private readonly AuthenticationDatabaseContext _context;

        public UserRepository(AuthenticationDatabaseContext context) {
            _context = context;
        }

        public async Task CreateUserAsync(User user) {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindByIdAsync(Guid id) {
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateUserAsync(User user) {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public User GetHashedPassword(string username, string email) {
            User user = _context.Users
                   .FirstOrDefault(u => u.Username == username || u.Email == email);

            if (user == null)
                return null;

            return user;
        }

        public async Task<UserExistenceResult> FindByEmailOrUsernameAsync(string email, string username) {
            var result = new UserExistenceResult();

            result.EmailExists = await _context.Users.AnyAsync(u => u.Email == email);
            result.UsernameExists = await _context.Users.AnyAsync(u => u.Username == username);

            return result;
        }

        public User GetFullUserWithRolesAndPolicies(string username, string email) {
            return _context.Users
                .Include(u => u.Role) // Load user's single role
                    .ThenInclude(r => r.Policies)
                        .ThenInclude(rp => rp.Policy)
                .Include(u => u.Policies)
                    .ThenInclude(up => up.Policy)
                .FirstOrDefault(u => u.Username == username || u.Email == email);
        }

        public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;

            if (user.RoleId == roleId)
                return true;

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
            if (!roleExists)
                return false;

            user.AssignRole(roleId);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<RoleDto> GetUserRoleAsync(Guid userId) {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Role == null)
                return null;

            return new RoleDto {
                Id = user.Role.Id,
                Name = user.Role.Name
            };
        }

        
        public async Task<bool> AssignUserPoliciesAsync(Guid userId, List<Guid> policyIds) {
            var validPolicies = await _context.Policies
                .Where(p => policyIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            foreach (var pid in validPolicies) {
                bool alreadyAssigned = await _context.UserPolicies.AnyAsync(up => up.UserId == userId && up.PolicyId == pid);
                if (!alreadyAssigned) {
                    _context.UserPolicies.Add(new UserPolicy(userId, pid));
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveUserPolicyAsync(Guid userId, Guid policyId) {
            var record = await _context.UserPolicies
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PolicyId == policyId);

            if (record == null)
                return false;

            _context.UserPolicies.Remove(record);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<User> GetUserWithPoliciesAndRoleAsync(Guid userId) {
            return await _context.Users
                .Include(u => u.Policies)
                    .ThenInclude(up => up.Policy)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> SuperAdminExistsAsync() {
            return await _context.Users
                .Include(u => u.Role)
                .AnyAsync(u => u.Role.Name == "SuperAdmin");
        }
    }
}
