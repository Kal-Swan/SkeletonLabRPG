using System.Net;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Diagnostics;
using SkeletonLabRpg.Common.Exceptions;

namespace SkeletonLabRpg.Api.Exceptions;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    TelemetryClient telemetryClient,
    IHostEnvironment environment)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        if (!environment.IsDevelopment())
        {
            telemetryClient.TrackException(new ExceptionTelemetry(exception));
        }
        ApiErrorResponse response;
        httpContext.Response.ContentType = "application/json";

        switch (exception)
        {
            case DatabaseException databaseException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ApiErrorResponse(HttpStatusCode.InternalServerError, ExceptionType.Database,"Failed to update changes");
                break;
            case MachineLearningException machineLearningException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ApiErrorResponse(HttpStatusCode.BadRequest, ExceptionType.Prediction,"Prediction engine failed");
                break;
            case BadRequestException badRequestException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new ApiErrorResponse(HttpStatusCode.BadRequest, ExceptionType.BadRequest,
                    badRequestException is { ShowCustomMessage: true, Message: not null } 
                        ? badRequestException.Message 
                        : "Request failed validation");
                break;
            case NotFoundException notFoundException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new ApiErrorResponse(HttpStatusCode.NotFound, ExceptionType.NotFound, 
                    notFoundException is { ShowCustomMessage: true, Message: not null} ? notFoundException.Message : "Saved item not found");
                break;
            default:
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new ApiErrorResponse(HttpStatusCode.InternalServerError, ExceptionType.Unknown,"An internal server error occurred");
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await httpContext.Response.WriteAsync(jsonResponse, cancellationToken);
        return true;
    }
}