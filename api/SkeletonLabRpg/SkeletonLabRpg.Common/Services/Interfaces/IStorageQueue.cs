namespace SkeletonLabRpg.Common.Services.Interfaces;

public interface IStorageQueue<in T> where T : class
{
    Task SendMessageAsync(IEnumerable<T> items);
}