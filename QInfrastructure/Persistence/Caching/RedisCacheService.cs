using System.Text.Json;
using QApplication.Caching;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace QInfrastructure.Persistence.Caching;

public class RedisCacheService: ICacheService
{
    
    private readonly IDatabase _db;
    

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false
    };
    
    
    public RedisCacheService( IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public  async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>(value!, SerializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, SerializerOptions);
        await _db.StringSetAsync(key, json, absoluteExpiration);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null,
        TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var value = await factory();
        if (value is null)
            return default;

        await SetAsync(key, value, absoluteExpiration, slidingExpiration, cancellationToken);
        return value;
    }

    public  async Task<T?> HashGetAsync<T>(string key, string field, CancellationToken ct = default)
    {
        var value = await _db.HashGetAsync(key, field);
        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>(value!, SerializerOptions);
    }

    public async Task HashSetAsync<T>(string key, string field, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value, SerializerOptions);
        await _db.HashSetAsync(key, field, json);

        if (expiry.HasValue)
        {
            await _db.KeyExpireAsync(key, expiry);
        }
    }

    public async Task HashRemoveAsync(string key, CancellationToken ct = default)
    {
        await _db.KeyDeleteAsync(key);
    }
}