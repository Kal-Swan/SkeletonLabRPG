using System.Text.Json;

namespace SkeletonLabRpg.Api.Llm.External;

public class LlmResponse
{
    public IEnumerable<JsonDocument> Builds {get; set; }
}