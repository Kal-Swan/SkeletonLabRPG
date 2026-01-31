using Microsoft.AspNetCore.SignalR;
using SkeletonLabRpg.Api.Authorization;
using SkeletonLabRpg.Common.Authorisation;

namespace SkeletonLabRpg.Api.SignalR;

public class UserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var azureOid = connection.User.FindFirst(ClaimConstants.AzureIdentityObjectIdClaimType)!.Value;
        return azureOid;
    }
}