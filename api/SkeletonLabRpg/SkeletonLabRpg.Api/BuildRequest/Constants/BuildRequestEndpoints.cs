using SkeletonLabRpg.Api.Endpoints;

namespace SkeletonLabRpg.Api.BuildRequest.Constants;

public static class BuildRequestEndpoints
{
    private const string Base = $"{EndpointConstants.BaseEndpoint}/buildrequest";

    public const string Create = $"{Base}/create";
    
    public const string Notify = $"{Base}/notify/{{id:guid}}";
    
    public const string GetBuildRequestDetails = $"{Base}/details";

    public const string UpdateBuildRequest = $"{Base}/update/{{id:guid}}";
    
    public const string Progression = $"{Base}/progress/{{id:guid}}";
}