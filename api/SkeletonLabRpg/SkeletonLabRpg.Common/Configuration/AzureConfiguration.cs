using Azure.Core;
using Azure.Identity;

namespace SkeletonLabRpg.Common.Configuration;

public class AzureConfiguration
{
    public TokenCredential Credential(string clientId) =>

    #if DEBUG
        new DefaultAzureCredential();
    #else 
    
       new ManagedIdentityCredential(clientId);
    #endif
    
}