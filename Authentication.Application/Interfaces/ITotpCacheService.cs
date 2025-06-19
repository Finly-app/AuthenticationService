public interface ITotpCacheService {
    Task SetTempSecretAsync(Guid userId, string secret, TimeSpan ttl);
    Task<string?> GetTempSecretAsync(Guid userId);
    Task RemoveTempSecretAsync(Guid userId);
}
