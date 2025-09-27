using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models.RpgBuild;

public class RpgSystemModel : CosmosDbDocumentBase<RpgSystemModel>
{
    public string Name { get; set; }

    public override string PartitionKey => "RpgSystem";
}