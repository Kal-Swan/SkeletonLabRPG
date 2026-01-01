using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Common.Cache;

public class TaskCache<T> : ITaskCache<T>
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions
    {
        ExpirationScanFrequency = TimeSpan.FromMinutes(10)
    });
    
    public async Task<T?> GetOrAdd(string key, TimeSpan timeToCache, Func<Task<T?>> task)
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            return value;
        }

        var currentValue = await task();
        
        if (currentValue is null)
        {
            return default;
        }
        
        _cache.Set(key, currentValue, timeToCache);

        return currentValue;
    }

    public async Task<IEnumerable<T>> GetOrAddMany(string key, TimeSpan timeToCache, Func<Task<List<T>>> task)
    {
        if (_cache.TryGetValue(key, out IEnumerable<T>? values))
        {
            return values!;
        }

        var currentValues = await task();

        if (currentValues.Count == 0)
        {
            return [];
        }
        
        _cache.Set(key, currentValues, timeToCache);

        return currentValues;
    }

    public void Invalidate(string? emailAccountForCollectionCached, string? singleCachedKey)
    {
        if (emailAccountForCollectionCached is not null)
        {
            var multiCacheKey = CacheKeys.GetRepositoryGetManyByType<T>(emailAccountForCollectionCached);
            _cache.Remove(multiCacheKey);
        }

        if (singleCachedKey is not null)
        {
            _cache.Remove(singleCachedKey);
        }
    }
}