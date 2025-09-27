using System.Text.Json;
using System.Text.Json.Serialization;
using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models.RpgBuild;

public class Build : CosmosDbDocumentBase<Build>
{
    public Guid RpgSystemId { get; set; }
    public string Name { get; set; }
    
    public string Reason { get; set; }
    public string Template { get; set; }
}