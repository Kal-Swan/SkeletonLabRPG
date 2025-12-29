using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;
using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Common.Database.Models.BuildRequest;

public class BuildRequestModel : CosmosDbDocumentBase<BuildRequestModel>
{
    public required BuildRequestStatus Status { get; set; }
    public required string Question { get; set; }
    public IEnumerable<BuildAnswer> Answers { get; set; }
    public required Guid BuildSystemId { get; set; }
    public override string ContainerName { get; } = "BuildRequests";
}