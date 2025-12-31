using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models.BuildRequest;

public class ChainBuildRequestsModel: CosmosDbDocumentBase<ChainBuildRequestsModel>
{
    public Guid BuildSystemId { get; set; }
    public IEnumerable<Guid> BuildRequestIds { get; set; }
}