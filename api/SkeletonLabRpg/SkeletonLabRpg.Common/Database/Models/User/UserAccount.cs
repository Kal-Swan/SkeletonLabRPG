using SkeletonLabRpg.Common.Database.Cosmosdb.Constants;
using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;

namespace SkeletonLabRpg.Common.Database.Models.User;

public class UserAccount : CosmosDbDocumentBase
{
    public string AzureOID { get; set; }
    
    public string Email { get; set; }
    
    public IEnumerable<string> Roles { get; set; }
}