public interface IPolicyService {
    Task<IEnumerable<PolicyDto>> GetAllAsync();
    Task<PolicyDto> GetByIdAsync(Guid id);
    Task CreateAsync(PolicyDto dto);
    Task UpdateAsync(PolicyDto dto);
    Task DeleteAsync(Guid id);
}