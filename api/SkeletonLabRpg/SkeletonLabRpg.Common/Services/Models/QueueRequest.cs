using System.Text.Json.Serialization;

namespace SkeletonLabRpg.Common.Services.Models;

public record QueueRequest(Guid Id, string Question, string BuildSystem)
{
    [JsonPropertyName("build_system")] public string BuildSystem { get; set; } = BuildSystem;
    [JsonPropertyName("question")] public string Question { get; set; } = Question;
    [JsonPropertyName("id")] public Guid Id { get; set; } = Id;
}