using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;
using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Common.Database.Models.Build;

public class BuildRequestModel : CosmosDbDocumentBase
{
    public BuildRequestStatus Status { get; set; }
    public string Question { get; set; }
    public IEnumerable<BuildAnswer> Answers { get; set; } = [];
    public Guid BuildSystemId { get; set; }
    public bool IsDeleted => Status == BuildRequestStatus.Deleted;
    public override string ContainerName => "BuildRequests";
}