using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace QAggregationService.Application.Caching;

public class MemoryCacheService : IMemoryCacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }


    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T value))
        {
            return value;
        }

        value = await factory();

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
        };

        _cache.Set(key, value, options);

        return value;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
    
}