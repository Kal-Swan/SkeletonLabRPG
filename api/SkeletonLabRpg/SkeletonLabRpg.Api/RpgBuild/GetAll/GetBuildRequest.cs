namespace SkeletonLabRpg.Api.RpgBuild.GetAll;

public class GetBuildRequest
{
    public Guid Id { get; set; }
    public Guid RpgSystemId { get; set; }
    public string Name { get; set; }
    public string Reason { get; set; }
    public string Template { get; set; }
}