public interface IRoleRepository {
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> GetByIdAsync(Guid id);
    Task CreateAsync(Role role);
    Task UpdateAsync(Role role);
    Task DeleteAsync(Guid id);
}