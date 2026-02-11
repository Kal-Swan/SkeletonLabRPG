using System.Security.Authentication;
using System.Security.Claims;
using SkeletonLabRpg.Api.Authorization;
using SkeletonLabRpg.Api.Extensions;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.User;

namespace SkeletonLabRpg.Api.Middlewares;

public class AccountEnrichmentMiddleware(RequestDelegate next)
{
    private const string HeaderUserIdName = "User-Id";
    private const string HeaderAoidName = "Azure-OId";
    public async Task InvokeAsync(HttpContext context, 
        AccountDetails accountDetails, 
        IRepository<UserAccount> userAccountRepository,
        IMemoryCache<UserAccount> memoryCache,
        ILogger<AccountEnrichmentMiddleware> logger)
    {
        if (context.Request.Headers.TryGetValue(HeaderUserIdName, out var userId) 
            && Guid.TryParse(userId, out var parsedGuidUserId) 
            && context.Request.Headers.TryGetValue(HeaderAoidName, out var llmAzureOId) 
            && !string.IsNullOrEmpty(llmAzureOId))
        {
            accountDetails.UserId = parsedGuidUserId;
            accountDetails.AzureIdentityObjectId = llmAzureOId;
        }
        
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                var azureOId = context.User.FindFirst(ClaimConstants.AzureIdentityObjectIdClaimType)?.Value;
                var userAccount = await memoryCache.GetOrAdd(
                    key: azureOId,
                    timeToCache: TimeSpan.FromMinutes(10),
                    task: async () =>
                    {
                        var newUser = new UserAccount
                        {
                            Email = context.User.FindFirst("preferred_username")?.Value,
                            AzureOID = azureOId,
                            Roles = context.User.Claims.GetRoles()
                        };
                        var createdUser = await userAccountRepository.GetOrCreateAsync(newUser, account => account.AzureOID == newUser.AzureOID);
                        return createdUser;
                    });
                accountDetails.UserId = userAccount!.Id;
                accountDetails.AzureIdentityObjectId = azureOId;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
                throw;
            }
        }
        await next(context);
    }
}