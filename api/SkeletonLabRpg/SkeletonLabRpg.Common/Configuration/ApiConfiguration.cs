namespace SkeletonLabRpg.Common.Configuration;

public class ApiConfiguration : AzureConfiguration
{
    public const string Name = "Configuration";
    
    public string AzureAppConfigurationEndpoint { get; set; }
    
    public string LlmEndpoint { get; set; }
}