using Newtonsoft.Json;
using SkeletonLabRpg.Common.Database.Cosmosdb.Constants;

namespace SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

public abstract class CosmosDbDocumentBase<T> : ICosmosDbDocumentBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [JsonProperty("partitionKey")] 
    public string PartitionKey => typeof(T).Name;

    [JsonProperty("_etag")]
    public string ETag { get; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Modified { get; set; }

    public required string AccountEmail { get; set; }

    [JsonIgnore]
    public virtual string ContainerName { get; } = ContainerNameConstants.DefaultContainer;
}