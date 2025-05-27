public interface IRoleService {
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<RoleDto> GetByIdAsync(Guid id);
    Task CreateAsync(RoleDto dto);
    Task UpdateAsync(RoleDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> AssignPoliciesToRoleAsync(Guid roleId, List<Guid> policyIds);
}