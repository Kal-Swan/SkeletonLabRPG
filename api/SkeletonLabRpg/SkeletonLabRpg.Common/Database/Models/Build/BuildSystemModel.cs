using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;
using SkeletonLabRpg.Common.Database.Cosmosdb.Enums;

namespace SkeletonLabRpg.Common.Database.Models.Build;

public class BuildSystemModel : CosmosDbDocumentBase
{
    public string Name { get; set; }
    
    public IEnumerable<string> FileNames { get; set; }
}