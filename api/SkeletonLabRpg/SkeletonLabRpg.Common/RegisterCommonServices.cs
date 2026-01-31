using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Configuration;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Models;
using SkeletonLabRpg.Common.Services;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Common;

public static class RegisterCommonServices
{
    public static IServiceCollection ConfigureCommonServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var cosmosDbConfiguration = configuration.GetSection(CosmosDbConfiguration.Name).Get<CosmosDbConfiguration>();
        services.AddSingleton(provider => new CosmosClient(cosmosDbConfiguration.Endpoint, new DefaultAzureCredential(), new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        }));
        services.AddScoped<ICosmosDbContainerFactory, CosmosDbContainerFactory>();
        services.AddScoped<UserContext>();
        services.AddScoped(typeof(IRepository<>), typeof(CosmosDbBaseRepository<>));
        services.AddScoped(typeof(UserScopedRepository<>));
        services.Configure<CosmosDbConfiguration>(configuration.GetSection(CosmosDbConfiguration.Name));
        var storageConfiguration = configuration.GetSection(StorageConfiguration.Name).Get<StorageConfiguration>();
        services.Configure<StorageConfiguration>(configuration.GetSection(StorageConfiguration.Name));
        services.AddSingleton(_ => new BlobServiceClient(new Uri(storageConfiguration!.Blob.Endpoint), new DefaultAzureCredential()));
        services.AddTransient<IBlobStorage, BlobStorage>();
        services.AddSingleton(typeof(IStorageQueue<>), typeof(QueueStorage<>));
        services.AddSingleton(typeof(IMemoryCache<>), typeof(MemoryCache<>));
        services.Configure<ServiceBusConfiguration>(configuration.GetSection(ServiceBusConfiguration.Name));
        var serviceBusConfiguration = configuration.GetSection(ServiceBusConfiguration.Name).Get<ServiceBusConfiguration>();
        services.AddSingleton(_ =>new ServiceBusClient(serviceBusConfiguration.Endpoint, new DefaultAzureCredential()));
        services.AddSingleton<IBuildRequestPublisher, BuildRequestPublisher>();
        return services;
    }
    
    public static IServiceCollection RegisterApplicationInsights(this IServiceCollection services, WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .CreateLogger();

            builder.Host.UseSerilog();
        }
        else
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.ApplicationInsights(new TelemetryConfiguration
                    {
                        ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
                    },
                    TelemetryConverter.Traces)
                .CreateLogger();

            builder.Host.UseSerilog();
        }
        services.AddApplicationInsightsTelemetry();
        
        return services;
    }
}