using Microsoft.Extensions.Logging;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;
using SkeletonLabRpg.Common.Database.Models;

namespace SkeletonLabRpg.Common.Database.Cosmosdb;

public class UserScopedRepository<T>
    (
        ICosmosDbContainerFactory cosmosDbContainerFactory, 
        IMemoryCache<T> memoryCache, 
        ILogger<CosmosDbBaseRepository<T>> logger, 
        AccountDetails accountDetails)
    : CosmosDbBaseRepository<T>(cosmosDbContainerFactory, memoryCache, logger, accountDetails.UserIdExists 
        ? new UserContext { UserId = accountDetails.UserId } : 
        new UserContext()) where T : CosmosDbDocumentBase, new();