using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces {
    public interface IUserRepository {
        Task CreateUserAsync(User user);
        Task<User> FindByIdAsync(Guid id);
        Task UpdateUserAsync(User user);

        User GetHashedPassword(string username, string email);
        Task<UserExistenceResult> FindByEmailOrUsernameAsync(string email, string username);
        User GetFullUserWithRolesAndPolicies(string? username, string? email);
        Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId);
        Task<RoleDto> GetUserRoleAsync(Guid userId);
        Task<bool> UpdateUserRoleAsync(Guid userId, Guid roleId);
        Task<User> GetUserWithPoliciesAndRoleAsync(Guid userId);
        Task<bool> AssignUserPoliciesAsync(Guid userId, List<Guid> policyIds);
        Task<bool> RemoveUserPolicyAsync(Guid userId, Guid policyId);
    }
}
