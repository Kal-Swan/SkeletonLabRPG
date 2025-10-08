using Azure.Identity;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using SkeletonLabRpg.Common;
using SkeletonLabRpg.Common.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var appConfiguration = builder.Configuration.GetSection(ApiConfiguration.Name).Get<ApiConfiguration>();

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(appConfiguration.AzureAppConfigurationEndpoint), new DefaultAzureCredential())
        .Select(KeyFilter.Any, LabelFilter.Null)
        .Select(KeyFilter.Any, builder.Environment.EnvironmentName);
});

builder.Services.ConfigureCommonServices(builder.Configuration);
builder.Build().Run();