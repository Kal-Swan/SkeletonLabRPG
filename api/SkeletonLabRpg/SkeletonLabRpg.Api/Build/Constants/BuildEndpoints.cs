using SkeletonLabRpg.Api.Endpoints;

namespace SkeletonLabRpg.Api.Build.Constants;

public static class BuildEndpoints
{
    private const string Base = $"{EndpointConstants.BaseEndpoint}/build";

    public const string GetAllRpgBuilds = $"{Base}/getall";

    public const string DeleteRpgBuild = $"{Base}/{{id:guid}}";
    
    public const string UpdateRpgBuild = $"{Base}/{{id:guid}}";
    
    public const string CreateRpgBuild = $"{Base}/create";
}