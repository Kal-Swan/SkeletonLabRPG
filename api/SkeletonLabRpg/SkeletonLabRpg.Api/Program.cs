using System.Security.Claims;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Identity.Web;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.Exceptions;
using SkeletonLabRpg.Api.Llm.External;
using SkeletonLabRpg.Api.Middlewares;
using SkeletonLabRpg.Common;
using SkeletonLabRpg.Common.Configuration;
using SkeletonLabRpg.Common.Constants;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpoints();

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection(AzureAuthConfiguration.Name));
builder.Services.AddAuthorization(options =>
{
    foreach (var role in AzureAdB2CClaimConstants.Roles)
    {
        options.AddPolicy(role, policy => policy.RequireClaim(ClaimTypes.Role, role));
    }
});

// builder.Services.AddTransient<IClaimsTransformation, RoleClaimsTransformer>();
builder.Services.AddScoped<AccountDetails>();
builder.Services.RegisterApplicationInsights(builder);
builder.Services.ConfigureCommonServices(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.Configure<SkeletonLabRpgConfiguration>(builder.Configuration.GetSection(SkeletonLabRpgConfiguration.Name));
builder.Services.AddCors();
//builder.Services.AddControllers();

var app = builder.Build();

app.UseExceptionHandler(_ => {});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
    //app.UseMiddleware<ExceptionLoggingMiddleware>();
}


app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthentication();
app.UseMiddleware<TestMiddleware>();
app.UseAuthorization();
app.UseMiddleware<ScopeAuthorisationMiddleware>();
app.UseMiddleware<AccountEnrichmentMiddleware>();

// app.UseRouting();
// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
// });
app.MapEndpoints();

app.Run();