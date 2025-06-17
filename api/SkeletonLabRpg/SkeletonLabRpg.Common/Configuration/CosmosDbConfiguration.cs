namespace SkeletonLabRpg.Common.Configuration;

public class CosmosDbConfiguration
{
    public const string Name = "Cosmosdb";
    
    public string ConnectionString { get; set; }
    
    public string Endpoint { get; set; }
    
    public string DatabaseName { get; set; }
    
    public string PartitionKeyPath { get; set; }
}