using SkeletonLabRpg.Common.Database.Models.BuildRequest;

namespace SkeletonLabRpg.Api.BuildRequest.Models;

public class LlmBuilds
{
    public IEnumerable<BuildAnswer> Builds { get; set; }
}