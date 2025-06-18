namespace SkeletonLabRpg.Common.Configuration;

public class AzureAuthConfiguration
{
    public const string Name = "AzureAuth";
    public string Instance { get; set; }
    public string ClientId { get; set; }
    public string Domain { get; set; }
    public string TenantId { get; set; }
    public string Audience { get; set; }
}