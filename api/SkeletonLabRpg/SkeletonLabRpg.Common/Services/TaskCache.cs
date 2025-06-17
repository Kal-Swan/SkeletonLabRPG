using Microsoft.Extensions.Caching.Memory;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Common.Services;

public class TaskCache<T> : ITaskCache<T>
{
    public async Task<T?> GetOrAdd(string key, TimeSpan timeToCache, Func<Task<T>> task)
    {
        var cache = new MemoryCache(new MemoryCacheOptions
        {
            ExpirationScanFrequency = timeToCache
        });

        if (cache.TryGetValue(key, out T? value))
        {
            return value;
        }

        var currentValue = await task();
        
        cache.Set(key, currentValue);

        return currentValue;
    }
}