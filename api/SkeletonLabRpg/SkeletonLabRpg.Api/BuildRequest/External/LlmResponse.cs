using System.Text.Json;

namespace SkeletonLabRpg.Api.BuildRequest.External;

public class LlmResponse
{
    public IEnumerable<JsonDocument> Builds {get; set; }
}