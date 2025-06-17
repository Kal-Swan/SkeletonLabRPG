using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models;

public class Character : CosmosDbDocumentBase<Character>
{
    public string Name { get; set; } = string.Empty;
}