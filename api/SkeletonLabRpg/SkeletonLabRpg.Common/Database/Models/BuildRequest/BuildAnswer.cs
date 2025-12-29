using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Common.Database.Models.BuildRequest;

public class BuildAnswer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Reason { get; set; }
    public string Template { get; set; }
    public BuildAnswerStatus Status { get; set; }
}