using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SkeletonLabRpg.Common.Configuration;

namespace SkeletonLabRpg.Common.Database.Cosmosdb;

public class CosmosDbContainerFactory(CosmosClient cosmosClient, IOptions<CosmosDbConfiguration> cosmosDbConfiguration) :
    ICosmosDbContainerFactory
{
    private static readonly SemaphoreSlim CacheLock = new SemaphoreSlim(1, 1);
    public async Task<Container> GetContainer(string containerName)
    {
        await InitializeContainer(containerName);
        return cosmosClient.GetContainer(cosmosDbConfiguration.Value.DatabaseName, containerName);
    }

    private async Task InitializeContainer(string containerName)
    {
        await CacheLock.WaitAsync();
        
        var cosmosDbInitializer = new CosmosDbInitializer(cosmosDbConfiguration.Value, cosmosClient);

        try
        {
            await cosmosDbInitializer.InitializeIfRequiredAsync(containerName);
        }
        finally
        {
            CacheLock.Release();
        }
        
    }
}