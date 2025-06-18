namespace SkeletonLabRpg.Common.Configuration;

public class AppConfiguration : AzureConfiguration
{
    public const string Name = "AppConfiguration";
    
    public string Endpoint { get; set; }
}