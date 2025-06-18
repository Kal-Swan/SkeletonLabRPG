using SkeletonLabRpg.Api.Endpoints;

namespace SkeletonLabRpg.Api.Character.Constants;

public static class CharacterEndpoints
{
    private const string Base = $"{EndpointConstants.BaseEndpoint}/character";

    public const string CreateCharacter = $"{Base}";
    
    public const string GetAllCharacters = $"{Base}/getAll";
    
    public const string GetCharacter = $"{Base}/{{id:guid}}";
    
    public const string UpdateCharacter = $"{Base}/{{id:guid}}";
    
    public const string DeleteCharacter = $"{Base}/{{id:guid}}";
    
    public const string DeleteAllCharacters = $"{Base}/deleteAll";
    
    public const string UpdateCharacterName = $"{Base}/{{id:guid}}/updateName";
}