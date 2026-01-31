namespace SkeletonLabRpg.Common.Authorisation;

public class AccountDetails
{
    
    public string Email { get; set; }
    
    public IEnumerable<string> Roles { get; set; }
    
    public string AzureIdentityObjectId { get; set; }
    
    public Guid UserId { get; set; }

    public bool UserIdExists => Guid.Empty != UserId;
}