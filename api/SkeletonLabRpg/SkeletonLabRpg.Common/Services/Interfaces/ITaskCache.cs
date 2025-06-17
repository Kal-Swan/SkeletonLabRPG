namespace SkeletonLabRpg.Common.Services.Interfaces;

public interface ITaskCache<T>
{
    Task<T?> GetOrAdd(string key, TimeSpan timeToCache, Func<Task<T>> task);
}