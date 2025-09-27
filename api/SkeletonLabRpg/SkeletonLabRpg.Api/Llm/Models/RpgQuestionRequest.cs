using System.Text.Json.Serialization;

namespace SkeletonLabRpg.Api.Llm.Models;

public class RpgQuestionRequest
{
    [JsonPropertyName("rpg_system")]
    public string RpgSystem { get; set; }

    [JsonPropertyName("question")]
    public string Question { get; set; }
}