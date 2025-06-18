using System.Text.Json;
using Azure.Identity;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using SkeletonLabRpg.Common.Configuration;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Common.Services;

public class QueueStorage<T>(IOptions<StorageConfiguration> storageConfiguration) : IStorageQueue<T> where T : class
{
    private readonly QueueClient queueClient = new(new Uri($"{storageConfiguration.Value.Queue.Endpoint}{storageConfiguration.Value.Queue.CharacterAttributeQueue.Name}"), new DefaultAzureCredential());

    public async Task SendMessageAsync(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            var jsonMessage = JsonSerializer.Serialize(item);
            await queueClient.SendMessageAsync(jsonMessage);
        }
    }
}