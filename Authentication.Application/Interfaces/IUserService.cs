using Authentication.Domain.Entities;

namespace Authentication.Application.Interfaces {
    public interface IUserService {
        Task<User> FindByIdAsync(Guid userId);
        Task GenerateEmailConfirmationAsync(User user);
        Task UpdateUserAsync(User user);
        Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId);
        Task<RoleDto?> GetUserRoleAsync(Guid userId);
        Task<bool> UpdateUserRoleAsync(Guid userId, Guid roleId);
        Task<List<PolicyDto>> GetUserPoliciesAsync(Guid userId);
        Task<bool> AssignUserPoliciesAsync(Guid userId, List<Guid> policyIds);
        Task<bool> RemoveUserPolicyAsync(Guid userId, Guid policyId);

    }
}
