using SkeletonLabRpg.Api.Extensions;

namespace SkeletonLabRpg.Api.Middlewares;

public class TestMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var scope = context.User.Claims.GetClaims("scope");
        
        await next(context);
    }
    
}