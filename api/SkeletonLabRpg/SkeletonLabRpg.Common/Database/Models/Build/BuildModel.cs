using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models.Build;

public class BuildModel : CosmosDbDocumentBase<BuildModel>
{
    public Guid BuildSystemId { get; set; }
    public string Name { get; set; }
    
    public string Reason { get; set; }
    public string Template { get; set; }
    
    public Guid BuildRequestId { get; set; }
}