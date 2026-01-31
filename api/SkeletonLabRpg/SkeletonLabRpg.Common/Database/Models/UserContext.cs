namespace SkeletonLabRpg.Common.Database.Models;

public record UserContext
{
    public Guid? UserId { get; set; }
    
    public string AzureIdentityObjectId { get; set; }
}