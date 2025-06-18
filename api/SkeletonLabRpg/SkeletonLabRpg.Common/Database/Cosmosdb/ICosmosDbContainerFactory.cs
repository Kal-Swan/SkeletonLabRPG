using Microsoft.Azure.Cosmos;

namespace SkeletonLabRpg.Common.Database.Cosmosdb;

public interface ICosmosDbContainerFactory
{
    Task<Container> GetContainer(string containerName);
}