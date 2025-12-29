using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models.Build;

public class BuildSystemModel : CosmosDbDocumentBase<BuildSystemModel>
{
    public string Name { get; set; }

    public override string PartitionKey => "BuildSystem";
}