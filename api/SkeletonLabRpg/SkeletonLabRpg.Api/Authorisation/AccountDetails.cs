namespace SkeletonLabRpg.Api.Authorisation;

public class AccountDetails
{
    public string Name { get; set; }
    
    public string Email { get; set; }
    
    public IEnumerable<string> Roles { get; set; }
}