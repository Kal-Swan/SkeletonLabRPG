using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;
using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Common.Database.Models;

public class CharacterClass : CosmosDbDocumentBase<CharacterClass>
{
    public string Name { get; set; } = string.Empty;
    public ClassType ClassType { get; set; }
    public Guid CharacterId { get; set; }
}