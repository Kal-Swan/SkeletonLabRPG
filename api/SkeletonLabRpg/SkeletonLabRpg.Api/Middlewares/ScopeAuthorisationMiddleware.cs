using System.Net;
using SkeletonLabRpg.Api.Extensions;

namespace SkeletonLabRpg.Api.Middlewares;

public class ScopeAuthorisationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var scope = context.User.Claims.GetClaims("scope");
        if (!scope.Contains("access"))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }
        
        await next(context);
    }
    
}