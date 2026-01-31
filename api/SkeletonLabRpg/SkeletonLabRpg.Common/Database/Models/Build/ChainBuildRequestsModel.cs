using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models.Build;

public class ChainBuildRequestsModel: CosmosDbDocumentBase
{
    public Guid BuildSystemId { get; set; }
    public IEnumerable<Guid> BuildRequestIds { get; set; }
}