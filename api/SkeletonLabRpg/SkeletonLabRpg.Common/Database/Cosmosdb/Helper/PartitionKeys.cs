namespace SkeletonLabRpg.Common.Database.Cosmosdb.Helper;

public static class PartitionKeys
{
    public static string Generic<T>(Guid? userId)
    {
#if DEBUG
        return userId is null ? 
            $"{StripTextModelFromEntity(typeof(T).Name)}:DEBUG" : 
            $"{StripTextModelFromEntity(typeof(T).Name)}:{userId}:DEBUG";
#endif
        return userId is null ? 
            $"{StripTextModelFromEntity(typeof(T).Name)}" : 
            $"{StripTextModelFromEntity(typeof(T).Name)}:{userId}";
    }

    private static string StripTextModelFromEntity(string modelName)
    {
        return modelName.Replace("Model", "");
    }
}