using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Endpoints;

namespace SkeletonLabRpg.Api.Health;

public static class GetHealth
{
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("health", Handler);
            app.MapGet("health/appinsights", AppInsightsHandler);
            app.MapGet("health/error", ErrorHandler);
        }
    }

    private static IResult Handler()
    {
        return Results.Ok("Is Healthy");
    }
    
    private static IResult AppInsightsHandler([FromServices] TelemetryClient telemetryClient)
    {
        telemetryClient.TrackEvent("AppInsights Health Check");
        return Results.Ok("Test log sent to App insights!");
    }
    
    private static IResult ErrorHandler()
    {
        throw new Exception("Testing error handling");
    }
}