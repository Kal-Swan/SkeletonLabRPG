using Microsoft.Extensions.Options;
using SkeletonLabRpg.Api.Configuration;
using SkeletonLabRpg.Common.Authorisation;

namespace SkeletonLabRpg.Api.Filters;

public class WorkerKeyFilter(
    IOptions<WorkerApiConfiguration> options,
    AccountDetails accountDetails,
    ILogger<WorkerKeyFilter> logger) : IEndpointFilter
{
    private const string HeaderApiKeyName = "X-Worker-Api-Key";
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderApiKeyName, out var providedKey) ||
            providedKey != options.Value.Key || !accountDetails.UserIdExists)
        {
            logger.LogError("Error in class [WorkerKeyFilter] - Worker api key does not match or user id does not exist");
            return Results.Unauthorized();
        }

        return await next(context);
    }
}