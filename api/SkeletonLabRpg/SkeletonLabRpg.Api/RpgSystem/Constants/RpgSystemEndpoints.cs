using SkeletonLabRpg.Api.Endpoints;

namespace SkeletonLabRpg.Api.RpgSystem.Constants;

public static class RpgSystemEndpoints
{
    public const string Base = $"{EndpointConstants.BaseEndpoint}/rpgsystem";
    public const string Update = $"{Base}/{{id:guid}}";
    public const string Delete = $"{Base}/{{id:guid}}";
}