
namespace QApplication.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default);
    
    Task<T?> HashGetAsync<T>(string key, string field, CancellationToken ct = default);
    Task HashSetAsync<T>(string key, string field, T value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task HashRemoveAsync(string key, CancellationToken ct = default);
}