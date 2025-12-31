namespace SkeletonLabRpg.Common.Cache;

public static class CacheHelper
{
    public static string FormCacheKey(string type, string methodName)
    {
        return $"{type}-{methodName}";
    }
}