using Microsoft.Extensions.Options;
using SkeletonLabRpg.Api.Configuration;

namespace SkeletonLabRpg.Api.Filters;

public class WorkerKeyFilter(IOptions<WorkerApiConfiguration> options) : IEndpointFilter
{
    private const string HeaderName = "X-Worker-Api-Key";
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey) ||
            providedKey != options.Value.Key)
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}