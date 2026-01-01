using SkeletonLabRpg.Api.Build.GetAll;
using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Api.Build.Extensions;

public static class BuildExtensions
{
    public static BuildResponse ToBuildResponseDto(this BuildModel buildModel)
    {
        return new BuildResponse
        {
            Id = buildModel.Id,
            Reason = buildModel.Reason,
            Name = buildModel.Name,
            Template = buildModel.Template,
            BuildSystemId = buildModel.BuildSystemId
        };
    }
}