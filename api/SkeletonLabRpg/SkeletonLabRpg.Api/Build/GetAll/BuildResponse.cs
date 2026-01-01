namespace SkeletonLabRpg.Api.Build.GetAll;

public class BuildResponse
{
    public Guid Id { get; set; }
    public Guid BuildSystemId { get; set; }
    public string Name { get; set; }
    public string Reason { get; set; }
    public string Template { get; set; }
}