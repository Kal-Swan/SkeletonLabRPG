using Newtonsoft.Json;

namespace SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

public interface ICosmosDbDocumentBase
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
    public string PartitionKey { get; }

    [JsonProperty("_etag")]
    public string ETag { get; }

    public DateTimeOffset Created { get; set; }

    public DateTimeOffset Modified { get; set; }
    
    public string ContainerName { get; }
}