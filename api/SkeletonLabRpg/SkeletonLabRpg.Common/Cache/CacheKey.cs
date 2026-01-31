using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Common.Cache;

public static class CacheKey
{
    public static string Create(string partitionKey, CacheType cacheType) => $"{partitionKey}-{cacheType}";
}