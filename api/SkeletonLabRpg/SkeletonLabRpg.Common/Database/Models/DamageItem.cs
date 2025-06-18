using System.Text.Json.Serialization;
using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models;

public class DamageItem : ICosmosDbDocumentBase
{
    public Guid Id { get; set; }
    public string PartitionKey { get; }
    
    public string ETag { get; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Modified { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public float Damage { get; set; }
    
    [JsonIgnore]
    public string ContainerName => nameof(DamageItem);
}