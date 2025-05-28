public interface IRoleService {
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto> GetByIdAsync(Guid id);
    Task CreateAsync(RoleDto dto);
    Task UpdateAsync(RoleDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> AssignPoliciesToRoleAsync(Guid roleId, List<Guid> policyIds);
    Task<IEnumerable<PolicyDto>> GetAllInheritedPoliciesAsync(Guid roleId);
    Task<List<PolicyDto>> GetRolePoliciesAsync(Guid roleId);
    Task<bool> CreateRoleInheritanceAsync(Guid parentRoleId, Guid childRoleId);
    Task<RoleTreeDto> GetRoleTreeAsync(Guid rootRoleId);
    Task<List<Policy>> GetAllPoliciesForRoleAndInheritedAsync(Guid roleId);
    Task<bool> RemovePolicyFromRoleAsync(Guid roleId, Guid policyId);
    List<Policy> GetAllPoliciesForRoleAndInherited(Guid roleId);
}