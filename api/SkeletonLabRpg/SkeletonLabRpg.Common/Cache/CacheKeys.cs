using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Common.Cache;

public static class CacheKeys
{
    public static string GetRepositoryGetManyByType<T>(string accountEmail) => $"{typeof(T).Name}-GetMany-{accountEmail}";
}