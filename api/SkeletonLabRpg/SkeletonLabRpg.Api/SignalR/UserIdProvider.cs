using Microsoft.AspNetCore.SignalR;

namespace SkeletonLabRpg.Api.SignalR;

public class UserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var email = connection.User.FindFirst("preferred_username")!.Value;
        return email;
    }
}