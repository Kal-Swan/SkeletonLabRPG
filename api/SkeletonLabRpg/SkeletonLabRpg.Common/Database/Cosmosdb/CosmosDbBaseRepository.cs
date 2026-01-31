using System.Linq.Expressions;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;
using SkeletonLabRpg.Common.Exceptions;
using Microsoft.Azure.Cosmos.Linq;
using SkeletonLabRpg.Common.Database.Cosmosdb.Helper;
using SkeletonLabRpg.Common.Database.Models;

namespace SkeletonLabRpg.Common.Database.Cosmosdb;

public class CosmosDbBaseRepository<T>(
    ICosmosDbContainerFactory cosmosDbContainerFactory,
    IMemoryCache<T> memoryCache,
    ILogger<CosmosDbBaseRepository<T>> logger,
    UserContext userContext) 
    : IRepository<T> where T : CosmosDbDocumentBase, new()
{
    private Lazy<Task<Container>> GetContainer(string containerName) =>
        new(() => cosmosDbContainerFactory.GetContainer(containerName));
    
    private readonly string _partitionKey = PartitionKeys.Generic<T>(userContext.UserId);
    
    
    public async Task<T> Create(T entity)
    {
        try
        {
            memoryCache.Invalidate(_partitionKey);   
            var container = await GetContainer(new T().ContainerName).Value;
            entity.Created = DateTimeOffset.UtcNow;
            entity.PartitionKey = _partitionKey;
            var result = await container.CreateItemAsync(entity, partitionKey: new PartitionKey(_partitionKey));
            if (result.StatusCode != System.Net.HttpStatusCode.Created)
            {
                logger.LogError(
                    "Failed to create item in cosmos db with status code: {StatusCode} and activity id: {ActivityId}",
                    result.StatusCode, result.ActivityId);
                throw new DatabaseException("Failed to create item");
            }

            return result.Resource;
        }
        catch (CosmosException exception)
        {
            logger.LogError("Failed to create item in cosmos db with error: {Error}", exception);
            throw new DatabaseException("Failed to create item");
        }
    }

    public async Task<T?> GetById(Guid id)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        var result = await container.ReadItemAsync<T>(id.ToString(), new PartitionKey(_partitionKey));
        return result;
    }
    
    public async Task<T?> GetSingleByPredicate(Expression<Func<T, bool>> predicate)
    {
        var container = await GetContainer(new T().ContainerName).Value;

        var query = container
            .GetItemLinqQueryable<T>(
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionKey)
                })
            .Where(predicate)
            .Take(1)
            .ToFeedIterator();

        if (!query.HasMoreResults)
        {
            return null;
        }
        var response = await query.ReadNextAsync();
        return response.Resource.FirstOrDefault();

    }

    public async Task<IEnumerable<T>> GetManyByPredicate(Expression<Func<T, bool>> predicate)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        return await memoryCache.GetOrAddMany(
            key: CacheKey.Create(_partitionKey, CacheType.Many),
            timeToCache: TimeSpan.FromMinutes(10),
            task: async () =>
            {
                var iterator = container
                    .GetItemLinqQueryable<T>(requestOptions: new QueryRequestOptions
                    {
                        PartitionKey = new PartitionKey(_partitionKey)
                    }).Where(predicate).ToFeedIterator();

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
    
    public async Task<IEnumerable<T>> GetAll()
    {
        var container = await GetContainer(new T().ContainerName).Value;
        return await memoryCache.GetOrAddMany(
            key: CacheKey.Create(_partitionKey, CacheType.Many),
            timeToCache: TimeSpan.FromMinutes(10),
            task: async () =>
            {
                var iterator = container
                    .GetItemLinqQueryable<T>(requestOptions: new QueryRequestOptions
                    {
                        PartitionKey = new PartitionKey(_partitionKey)
                    }).ToFeedIterator();

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
        memoryCache.Invalidate(_partitionKey); 
        var container = await GetContainer(new T().ContainerName).Value;
        var items = await container.GetItemLinqQueryable<T>().Where(predicate).ToListAsync();
        foreach (var item in items)
        {
            await container.DeleteItemAsync<T>(item.Id.ToString(), new PartitionKey(_partitionKey));
        }
        return true;
    }

    public async Task<T> Update(T entity)
    {
        memoryCache.Invalidate(_partitionKey); 
        var container = await GetContainer(new T().ContainerName).Value;
        entity.Modified = DateTimeOffset.UtcNow;
        return await container.UpsertItemAsync(entity, partitionKey: new PartitionKey(entity.PartitionKey));
    }

    public async Task<bool> Delete(Guid id)
    {
        memoryCache.Invalidate(_partitionKey); 
        var container = await GetContainer(new T().ContainerName).Value;
        var deleted = await container.DeleteItemAsync<T>(id.ToString(), new PartitionKey(_partitionKey));
        return deleted.StatusCode == System.Net.HttpStatusCode.NoContent;
    }

    public async Task<T> Update(Guid id, T entity)
    {
        memoryCache.Invalidate(_partitionKey); 
        var container = await GetContainer(new T().ContainerName).Value;
        entity.Modified = DateTimeOffset.UtcNow;
        return await container.UpsertItemAsync(entity, partitionKey: new PartitionKey(entity.PartitionKey));
    }

    public async Task<T> GetOrCreateAsync(T entity, Expression<Func<T, bool>> predicate)
    {
        var container = await GetContainer(new T().ContainerName).Value;
        try
        {
            return await GetSingleByPredicate(predicate) ?? throw new CosmosException("Not Found", HttpStatusCode.NotFound, 0, string.Empty, 0);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            memoryCache.Invalidate(_partitionKey); 
            entity.Created = DateTimeOffset.UtcNow;
            entity.PartitionKey = _partitionKey;
            return await container.CreateItemAsync(entity, new PartitionKey(_partitionKey));
        }
    }
}