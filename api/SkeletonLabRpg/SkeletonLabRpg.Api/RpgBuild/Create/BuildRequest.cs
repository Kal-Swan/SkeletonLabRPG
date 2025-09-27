namespace SkeletonLabRpg.Api.RpgBuild.Create;

public class BuildRequest
{
    public Guid RpgSystemId { get; set; }
    public string Name { get; set; }
    
    public string Reason { get; set; }
    public string  Template { get; set; }
}