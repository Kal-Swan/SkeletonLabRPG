namespace SkeletonLabRpg.Api.Build.Create;

public class BuildRequest
{
    public Guid BuildSystemId { get; set; }
    public string Name { get; set; }
    
    public string Reason { get; set; }
    public string  Template { get; set; }
}