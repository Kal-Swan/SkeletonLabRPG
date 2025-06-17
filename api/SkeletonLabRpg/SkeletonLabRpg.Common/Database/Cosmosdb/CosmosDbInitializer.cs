using Microsoft.Azure.Cosmos;
using SkeletonLabRpg.Common.Configuration;

namespace SkeletonLabRpg.Common.Database.Cosmosdb;

public class CosmosDbInitializer(CosmosDbConfiguration configuration, CosmosClient cosmosClient)
{
    private static readonly List<string> CollectionsInitialized = [];

    public async Task InitializeIfRequiredAsync(string containerName)
    {
        if (CollectionsInitialized.Contains(containerName))
        {
            return;
        }

        await InitializeCollectionIfNotExistsAsync(containerName);
    }

    private async Task InitializeCollectionIfNotExistsAsync(string containerName)
    {
        var createdDatabaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(configuration.DatabaseName);

        if (createdDatabaseResponse.StatusCode != System.Net.HttpStatusCode.Created &&
            createdDatabaseResponse.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception($"Failed to create database: {configuration.DatabaseName} with status code {createdDatabaseResponse.StatusCode}");
        }

        var containerResponse = await createdDatabaseResponse.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
        {
            Id = containerName,
            PartitionKeyPath = $"{configuration.PartitionKeyPath}",
        });

        if (containerResponse.StatusCode != System.Net.HttpStatusCode.Created &&
            containerResponse.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception($"Failed to create container: {containerName} with status code {containerResponse.StatusCode}");
        }
        
        CollectionsInitialized.Add(containerName);
    }
}