using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace SkeletonLabRpg.Api.Middlewares;

public class ExceptionLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TelemetryClient _telemetryClient;

    public ExceptionLoggingMiddleware(RequestDelegate next, TelemetryClient telemetryClient)
    {
        _next = next;
        _telemetryClient = telemetryClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _telemetryClient.TrackException(new ExceptionTelemetry(ex));
            throw;
        }
    }
}