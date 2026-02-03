using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SkeletonLabRpg.Api.BuildRequest.Constants;
using SkeletonLabRpg.Api.BuildRequest.Models;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.Filters;
using SkeletonLabRpg.Api.SignalR;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.User;
using BuildRequestModel = SkeletonLabRpg.Common.Database.Models.Build.BuildRequestModel;

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
            [FromRoute] Guid id,
            [FromBody] LlmBuilds llmBuilds,
            [FromServices] UserScopedRepository<BuildRequestModel> repository,
            [FromServices] UserScopedRepository<BuildSystemModel> buildSystemRepository,
            [FromServices] IRepository<UserAccount> userAccountRepository,
            [FromServices] IHubContext<BuildHub> buildHubContext,
            [FromServices] ILogger<Endpoint> logger,
            [FromServices] AccountDetails accountDetails)
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
            
            var user = await userAccountRepository.GetById(accountDetails.UserId);
            
            logger.LogInformation("BuildRequestNotify: Notifying user by account details User Id {UserId} with Azure OID {AzureOID}", accountDetails.UserId, user.AzureOID);

            await buildHubContext.Clients.User(user.AzureOID).SendAsync("BuildCompleted", buildRequestResponse);

            return Results.Accepted();
        }
    }
}