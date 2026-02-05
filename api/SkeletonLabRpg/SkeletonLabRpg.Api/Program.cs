using System.Security.Claims;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Identity.Web;
using SkeletonLabRpg.Api.BuildRequest.External;
using SkeletonLabRpg.Api.Configuration;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.Exceptions;
using SkeletonLabRpg.Api.Middlewares;
using SkeletonLabRpg.Api.SignalR;
using SkeletonLabRpg.Common;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Configuration;
using SkeletonLabRpg.Common.Constants;

const string buildHubEndpointPath = "/buildHub";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpoints();
builder.Services.AddControllers();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = options.Limits.MaxRequestBodySize = 52_428_800;
});

var appConfiguration = builder.Configuration.GetSection(ApiConfiguration.Name).Get<ApiConfiguration>();
builder.Services.AddHttpClient<LlmService>(client =>
{
    client.BaseAddress = new Uri($"{appConfiguration.LlmEndpoint}/api/llm/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromMinutes(5);
});


builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(appConfiguration.AzureAppConfigurationEndpoint), new DefaultAzureCredential())
        .Select(KeyFilter.Any, LabelFilter.Null)
        .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
        .ConfigureClientOptions(options =>
        {
            options.Retry.NetworkTimeout = TimeSpan.FromSeconds(30);
        });
});

// var azureAdb2CConfiguration =
//     builder.Configuration.GetSection(AzureAuthConfiguration.Name).Get<AzureAuthConfiguration>();
var azureAdb2CConfiguration =
    builder.Configuration.GetSection(AzureAuthConfiguration.Name).Get<AzureAuthConfiguration>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder =>
    {

        builder.Audience = azureAdb2CConfiguration.Audience;
        builder.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments(buildHubEndpointPath)))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    }, options =>
    {
        options.ClientId = azureAdb2CConfiguration.ClientId;
        options.Domain = azureAdb2CConfiguration.Domain;
        options.TenantId = azureAdb2CConfiguration.TenantId;
        options.Instance = azureAdb2CConfiguration.Instance;
    });
builder.Services.AddAuthorization(options =>
{
    foreach (var role in AzureAdB2CClaimConstants.Roles)
    {
        options.AddPolicy(role, policy => policy.RequireClaim(ClaimTypes.Role, role));
    }
});

builder.Services.AddScoped<AccountDetails>();
builder.Services.ConfigureCommonServices(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.Configure<SkeletonLabRpgConfiguration>(builder.Configuration.GetSection(SkeletonLabRpgConfiguration.Name));
builder.Services.AddCors();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

var corsConfiguration =
    builder.Configuration.GetSection(CorsConfiguration.Name).Get<CorsConfiguration>();

builder.Services.Configure<WorkerApiConfiguration>(builder.Configuration.GetSection(WorkerApiConfiguration.Name));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins(corsConfiguration.Web).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

builder.Logging.ClearProviders();
builder.Services.RegisterApplicationInsights(builder);
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogCritical("STARTUP MARKER 2026-02-05-001");
logger.LogInformation("testing logs in program file");
logger.LogInformation("cors config web: {WEB}", corsConfiguration.Web);

app.UseExceptionHandler(_ => {});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseMiddleware<TestMiddleware>();
app.UseAuthorization();
app.UseMiddleware<ScopeAuthorisationMiddleware>();
app.UseMiddleware<AccountEnrichmentMiddleware>();

app.MapHub<BuildHub>(buildHubEndpointPath);
app.MapEndpoints();

app.Run();