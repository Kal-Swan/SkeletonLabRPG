namespace SkeletonLabRpg.Common.Configuration;

public class ServiceBusConfiguration
{
    public const string Name = "ServiceBus";
    
    public string Endpoint { get; set; }
    
    public string QueueName { get; set; }
}