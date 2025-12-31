using SkeletonLabRpg.Api.Endpoints;

namespace SkeletonLabRpg.Api.BuildSystem.Constants;

public static class BuildSystemEndpoints
{
    public const string Base = $"{EndpointConstants.BaseEndpoint}/buildsystem";
    public const string Update = $"{Base}/{{id:guid}}";
    public const string Delete = $"{Base}/{{id:guid}}";
}