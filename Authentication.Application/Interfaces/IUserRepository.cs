public interface IUserRepository {
    Task CreateUserAsync(User user);
    Task<User> FindByIdAsync(Guid id);
    Task UpdateUserAsync(User user);
    User GetHashedPassword(string username, string email);
    Task<UserExistenceResult> FindByEmailOrUsernameAsync(string email, string username);
    User GetFullUserWithRolesAndPolicies(string? username, string? email);
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId);
    Task<RoleDto> GetUserRoleAsync(Guid userId);
    Task<User> GetUserWithPoliciesAndRoleAsync(Guid userId);
    Task<bool> AssignUserPoliciesAsync(Guid userId, List<Guid> policyIds);
    Task<bool> RemoveUserPolicyAsync(Guid userId, Guid policyId);
    Task<bool> SuperAdminExistsAsync();
    Task<List<User>> GetAllUsersAsync();
    Task<List<User>> GetActiveUsersAsync();
    Task<List<User>> GetDeactivatedUsersAsync();
}