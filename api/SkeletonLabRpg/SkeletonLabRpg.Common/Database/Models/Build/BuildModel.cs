using SkeletonLabRpg.Common.Database.Cosmosdb.Constants;
using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models.Builds;

public class BuildModel : CosmosDbDocumentBase
{
    public Guid BuildSystemId { get; set; }
    public string Name { get; set; }
    
    public string Reason { get; set; }
    public string Template { get; set; }
    
    public Guid BuildRequestId { get; set; }
}