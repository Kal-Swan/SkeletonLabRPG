using SkeletonLabRpg.Api.Endpoints;

namespace SkeletonLabRpg.Api.Llm.Constants;

public static class LlmEndpoints
{
    private const string Base = $"{EndpointConstants.BaseEndpoint}/llm";

    public const string CreateRpgBuilds = $"{Base}/createrpgbuilds";

    public const string GetRpgSystems = $"{Base}/getrpgsystems";
    
    public const string GetAllRpgBuilds = $"{Base}/getallrpgbuilds";
}