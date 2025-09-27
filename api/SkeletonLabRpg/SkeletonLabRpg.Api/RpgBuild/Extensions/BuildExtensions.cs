using SkeletonLabRpg.Api.RpgBuild.GetAll;
using SkeletonLabRpg.Common.Database.Models.RpgBuild;

namespace SkeletonLabRpg.Api.RpgBuild.Extensions;

public static class BuildExtensions
{
    public static GetBuildRequest ToBuildRequest(this Build build)
    {
        return new GetBuildRequest
        {
            Id = build.Id,
            Reason = build.Reason,
            Name = build.Name,
            Template = build.Template,
            RpgSystemId = build.RpgSystemId
        };
    }
}