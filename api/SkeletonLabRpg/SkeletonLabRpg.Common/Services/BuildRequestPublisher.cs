using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using SkeletonLabRpg.Common.Configuration;
using SkeletonLabRpg.Common.Services.Interfaces;
using SkeletonLabRpg.Common.Services.Models;

namespace SkeletonLabRpg.Common.Services;

public class BuildRequestPublisher(ServiceBusClient client, IOptions<ServiceBusConfiguration> configuration)
    : IBuildRequestPublisher
{
    private readonly string _queueName = configuration.Value.QueueName;

    public async Task SendQueueAsync(QueueRequest buildRequestModel)
    {
        var sender = client.CreateSender(_queueName);

        var messageBody = JsonSerializer.Serialize(buildRequestModel);

        var message = new ServiceBusMessage(messageBody)
        {
            MessageId = buildRequestModel.BuildRequestId.ToString(), 
            ContentType = "application/json"
        };

        await sender.SendMessageAsync(message);
    }
}