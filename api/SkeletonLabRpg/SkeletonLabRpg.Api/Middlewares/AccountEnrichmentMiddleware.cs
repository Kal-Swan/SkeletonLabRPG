using DEXRPG.WebApi.Authorisation;
using SkeletonLabRpg.Api.Extensions;

namespace SkeletonLabRpg.Api.Middlewares;

public class AccountEnrichmentMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AccountDetails accountDetails)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            accountDetails.Email = context.User.FindFirst("preferred_username")?.Value;
            accountDetails.Name = context.User.FindFirst("name")?.Value;
            accountDetails.Roles = context.User.Claims.GetRoles();
        }
        await next(context);
    }
}