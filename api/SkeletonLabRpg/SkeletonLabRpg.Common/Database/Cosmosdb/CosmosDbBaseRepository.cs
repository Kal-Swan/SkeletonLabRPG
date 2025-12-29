using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;
using SkeletonLabRpg.Common.Exceptions;
using SkeletonLabRpg.Common.Services.Interfaces;
using Microsoft.Azure.Cosmos.Linq;

namespace SkeletonLabRpg.Common.Database.Cosmosdb;

public class CosmosDbBaseRepository<T>(ICosmosDbContainerFactory cosmosDbContainerFactory, ITaskCache<T> taskCache, ILogger<CosmosDbBaseRepository<T>> logger) 
    : IRepository<T> where T : class, ICosmosDbDocumentBase, new()
{
    private Lazy<Task<Container>> GetContainer(string containerName) =>
        new(() => cosmosDbContainerFactory.GetContainer(containerName));

    public async Task<T> Create(T entity)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        entity.Created = DateTimeOffset.UtcNow;
        var result = await container.CreateItemAsync(entity, partitionKey: new PartitionKey((new T()).PartitionKey));
        if (result.StatusCode != System.Net.HttpStatusCode.Created)
        {
            logger.LogError("Failed to create item in cosmos db with status code: {StatusCode} and activity id: {ActivityId}", result.StatusCode, result.ActivityId);
            throw new DatabaseException("Failed to create item");
        }
        return result.Resource;
    }

    public async Task<T?> GetById(Guid id)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        var result = await taskCache.GetOrAdd(
            id.ToString(),
            TimeSpan.FromMinutes(10),
            async () => await container.ReadItemAsync<T>(id.ToString(), new PartitionKey(new T().PartitionKey)));
        return result;
    }
    
    public async Task<T?> GetByPredicate(Expression<Func<T, bool>> predicate)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        return await container.GetItemLinqQueryable<T>(requestOptions: new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(new T().PartitionKey)
        }).FirstOrDefaultAsync(predicate);

    }

    public async Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> predicate, string accountEmail)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        return await taskCache.GetOrAddMany(
            key: CacheKeys.BuildSystemGetManyCacheKey(accountEmail),
            timeToCache: TimeSpan.FromMinutes(10),
            task: async () =>
            {
                var iterator = container
                    .GetItemLinqQueryable<T>(requestOptions: new QueryRequestOptions
                    {
                        PartitionKey = new PartitionKey(new T().PartitionKey)
                    })
                    .Where(predicate)
                    .ToFeedIterator();

                var results = new List<T>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    double ru = response.RequestCharge;
                    results.AddRange(response);
                }

                return results;
            });
    }

    public async Task<bool> DeleteMany(Expression<Func<T, bool>> predicate)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        var items = await container.GetItemLinqQueryable<T>().Where(predicate).ToListAsync();
        foreach (var item in items)
        {
            await container.DeleteItemAsync<T>(item.Id.ToString(), new PartitionKey(item.PartitionKey));
        }
        return true;
    }

    public async Task<T> Update(T entity)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        entity.Modified = DateTimeOffset.UtcNow;
        return await container.UpsertItemAsync(entity, partitionKey: new PartitionKey(entity.PartitionKey));
    }

    public async Task<bool> Delete(Guid id)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        var deleted = await container.DeleteItemAsync<T>(id.ToString(), new PartitionKey(new T().PartitionKey));
        return deleted.StatusCode == System.Net.HttpStatusCode.NoContent;
    }

    public async Task<T> Update(Guid id, T entity)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        entity.Modified = DateTimeOffset.UtcNow;
        return await container.UpsertItemAsync(entity, partitionKey: new PartitionKey(entity.PartitionKey));
    }
}