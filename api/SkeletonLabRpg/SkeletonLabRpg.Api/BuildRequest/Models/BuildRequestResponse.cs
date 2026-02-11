using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Api.BuildRequest.Models;

public record BuildRequestResponse(Guid Id, string Question, Guid BuildSystemId, string BuildSystemName, BuildRequestStatus Status, IEnumerable<BuildAnswer> Answers, DateTimeOffset LatestProcessedDate, int? Progression = null);
