using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using QApplication.Caching;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace QInfrastructure.Persistence.Caching;

public class RedisCacheService: ICacheService
{
    private readonly IDistributedCache _cache;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false
    };
    
    
    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public  async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await _cache.GetAsync(key, cancellationToken);
        if (bytes is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(bytes, SerializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null,
        CancellationToken cancellationToken = default)
    {
        var options= new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpiration,
            SlidingExpiration = slidingExpiration
        };

        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, SerializerOptions));
        await _cache.SetAsync(key, bytes, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var value = await factory();
        if (value is null)
        {
            return default;
        }

        await SetAsync(key, value, absoluteExpiration, slidingExpiration, cancellationToken);
        return value;
    }
}