using System.Text.Json.Serialization;
using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Api.BuildRequest.Models;

public class LlmBuilds
{
    public IEnumerable<BuildAnswer> Builds { get; set; }
    
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
}