using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Common.Cache;

public static class CacheKeys
{
    public static string BuildSystemGetManyCacheKey(string accountEmail) => $"{nameof(BuildSystemModel)}-GetMany-{accountEmail}";
}