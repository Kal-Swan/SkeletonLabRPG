using SkeletonLabRpg.Api.Endpoints;

namespace SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute.Constants;

public static class CharacterAttributeEndpoints
{
    private const string Base = $"{EndpointConstants.BaseEndpoint}/characterattribute/{{characterClassId:guid}}";

    public const string CreateCharacterAttribute = Base;
    
    public const string GetCharacterAttribute = Base;
}