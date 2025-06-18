using SkeletonLabRpg.Api.Endpoints;

namespace SkeletonLabRpg.Api.Character.CharacterClass.Constants;

public static class CharacterClassEndpoints
{
    private const string Base = $"{EndpointConstants.BaseEndpoint}/characterclass/{{characterId:guid}}";
    public const string CreateCharacterClass = $"{Base}";
    public const string GetCharacterClasses = $"{Base}/getall";
}