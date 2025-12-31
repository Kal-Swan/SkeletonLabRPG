using SkeletonLabRpg.Common.Services.Models;

namespace SkeletonLabRpg.Common.Services.Interfaces;

public interface IBuildRequestPublisher
{
    Task SendQueueAsync(QueueRequest queueRequest);
}