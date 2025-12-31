using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SkeletonLabRpg.Api.BuildRequest.Constants;
using SkeletonLabRpg.Api.BuildRequest.Models;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.Filters;
using SkeletonLabRpg.Api.SignalR;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.BuildRequest;
using BuildRequestModel = SkeletonLabRpg.Common.Database.Models.BuildRequest.BuildRequestModel;

namespace SkeletonLabRpg.Api.BuildRequest;

public static class BuildRequestNotify
{
    private record Response(Guid Id, string Question, Guid BuildSystemId, string BuildSystemName, BuildRequestStatus Status, IEnumerable<BuildAnswer> Answers, DateTimeOffset LatestProcessedDate);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost(BuildRequestEndpoints.Notify, Handler)
                .WithMetadata(new WorkerEndpointMetadata())
                .AddEndpointFilter<WorkerKeyFilter>();
        }
        
        private static async Task<IResult> Handler(
            [FromRoute]Guid id,
            [FromBody] LlmBuilds llmBuilds,
            [FromServices] IRepository<BuildRequestModel> repository,
            [FromServices] IRepository<BuildSystemModel> buildSystemRepository,
            [FromServices] IHubContext<BuildHub> buildHubContext,
            [FromServices] ILogger<Endpoint> logger)
        {
            var buildRequest = await repository.GetById(id);
            
            if (buildRequest is null)
            {
                logger.LogInformation("BuildRequestNotify: No build request found with id {ID}", id);
                return Results.BadRequest();
            }

            buildRequest.Status = BuildRequestStatus.Completed;
            buildRequest.Answers = llmBuilds.Builds.Select(build => new BuildAnswer
            {
                Id = Guid.NewGuid(),
                Name = build.Name,
                Template =  build.Template,
                Reason = build.Reason
            });
            
            await repository.Update(buildRequest);
            
            var buildSystem = await buildSystemRepository.GetById(buildRequest.BuildSystemId);

            var buildRequestResponse = new Response(
                buildRequest.Id,
                buildRequest.Question,
                buildRequest.BuildSystemId,
                buildSystem.Name,
                buildRequest.Status,
                buildRequest.Answers,
                buildRequest.Modified);

            await buildHubContext.Clients.User(buildRequest.AccountEmail).SendAsync("BuildCompleted", buildRequestResponse);

            return Results.Accepted();
        }
    }
}