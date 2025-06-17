using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models;

public class UserAccount : CosmosDbDocumentBase<UserAccount>
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public IEnumerable<string> Roles { get; set; }
}