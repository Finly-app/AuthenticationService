public interface IRoleRepository {
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> GetByIdAsync(Guid id);
    Task CreateAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(Guid id);
    Task<bool> AssignPoliciesToRoleAsync(Guid roleId, List<Guid> policyIds);
    Task<List<Policy>> GetAllInheritedPoliciesAsync(Guid roleId);
    Task<List<Policy>> GetRolePoliciesAsync(Guid roleId);
    Task<bool> CreateRoleInheritanceAsync(Guid parentRoleId, Guid childRoleId);
    Task<List<Guid>> GetChildRoleIdsAsync(Guid parentRoleId);
    Task<List<Policy>> GetPoliciesForRolesAsync(List<Guid> roleIds);
    Task<List<Guid>> GetAllDescendantRoleIdsAsync(Guid rootRoleId);
    Task<bool> RemovePolicyFromRoleAsync(Guid roleId, Guid policyId);
}