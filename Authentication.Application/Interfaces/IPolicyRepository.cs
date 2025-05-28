namespace Authentication.Application.Interfaces {
    public interface IPolicyRepository {
        Task<IEnumerable<Policy>> GetAllAsync();
        Task<Policy> GetByIdAsync(Guid id);
        Task CreateAsync(Policy dto);
        Task BulkCreatePolicies(List<Policy> policies);
        Task UpdateAsync(Policy dto);
        Task DeleteAsync(Guid id);
    }
}
