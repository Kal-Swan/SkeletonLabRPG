using System.Text.Json.Serialization;

namespace SkeletonLabRpg.Common.Services.Models;

public record QueueRequest(Guid UserId, Guid BuildRequestId, Guid BuildSystemId, string Question, string BuildSystem)
{
    [JsonPropertyName("user_id")] public Guid UserId { get; set; } = UserId;
    [JsonPropertyName("build_request_id")] public Guid BuildRequestId { get; set; } = BuildRequestId;
    [JsonPropertyName("build_system_id")] public Guid BuildSystemId { get; set; } = BuildSystemId;
    [JsonPropertyName("question")] public string Question { get; set; } = Question;
    [JsonPropertyName("build_system")] public string BuildSystem { get; set; } = BuildSystem;
}