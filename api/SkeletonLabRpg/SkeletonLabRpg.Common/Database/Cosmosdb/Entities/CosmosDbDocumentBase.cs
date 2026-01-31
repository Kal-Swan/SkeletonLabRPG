using Newtonsoft.Json;
using SkeletonLabRpg.Common.Database.Cosmosdb.Constants;
using SkeletonLabRpg.Common.Database.Cosmosdb.Enums;
using SkeletonLabRpg.Common.Database.Cosmosdb.Helper;

namespace SkeletonLabRpg.Common.Database.Cosmosdb.Entities;
public abstract class CosmosDbDocumentBase : ICosmosDbDocumentBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [JsonProperty("partitionKey")] 
    public virtual string PartitionKey { get; set; }

    [JsonProperty("_etag")]
    public string ETag { get; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Modified { get; set; }
    public virtual string ContainerName => ContainerNameConstants.DefaultContainer;

    public DateTimeOffset Deleted { get; set; }
    
    public DocumentStatus Status { get; set; }
}