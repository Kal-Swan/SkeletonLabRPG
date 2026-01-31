namespace SkeletonLabRpg.Common.Cache;

public interface IMemoryCache<T>
{
    Task<T?> GetOrAdd(string key, TimeSpan timeToCache, Func<Task<T?>> task);
    
    Task<IEnumerable<T>>  GetOrAddMany(string key, TimeSpan timeToCache, Func<Task<List<T>>> task);

    void Invalidate(string partitionKey);
}