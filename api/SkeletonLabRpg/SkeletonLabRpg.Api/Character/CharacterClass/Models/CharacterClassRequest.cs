using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Api.Character.CharacterClass.Models;

public class CharacterClassRequest
{
    public Guid CharacterId { get; set; }
    
    public ClassType ClassType { get; set; }
    
    public string Name { get; set; } = string.Empty;
}