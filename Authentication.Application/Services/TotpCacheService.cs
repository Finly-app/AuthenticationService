using Microsoft.Extensions.Caching.Memory;

public class TotpCacheService : ITotpCacheService {
    private readonly IMemoryCache _memoryCache;

    public TotpCacheService(IMemoryCache memoryCache) {
        _memoryCache = memoryCache;
    }

    private string GetKey(Guid userId) => $"2fa:setup:{userId}";

    public Task SetTempSecretAsync(Guid userId, string secret, TimeSpan ttl) {
        _memoryCache.Set(GetKey(userId), secret, ttl);
        return Task.CompletedTask;
    }

    public Task<string?> GetTempSecretAsync(Guid userId) {
        return Task.FromResult(
            _memoryCache.TryGetValue(GetKey(userId), out string? secret) ? secret : null
        );
    }

    public Task RemoveTempSecretAsync(Guid userId) {
        _memoryCache.Remove(GetKey(userId));
        return Task.CompletedTask;
    }
}
