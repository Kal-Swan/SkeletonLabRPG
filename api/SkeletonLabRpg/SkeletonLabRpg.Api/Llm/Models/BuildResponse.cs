using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkeletonLabRpg.Api.Llm.Models;

public class BuildResponse
{
    public IEnumerable<Build> Builds { get; set; }
}

public class Build
{
    public string Name { get; set; }
    public string Reason { get; set; }
    public string  Template { get; set; }
}