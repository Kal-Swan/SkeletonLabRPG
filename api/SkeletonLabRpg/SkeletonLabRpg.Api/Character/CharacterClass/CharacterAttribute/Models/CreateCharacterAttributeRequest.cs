namespace SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute.Models;

public class CreateCharacterAttributeRequest
{
    public CharacterAttributeRequest Favourite { get; set; }
    public IEnumerable<CharacterAttributeRequest> Others { get; set; }
}