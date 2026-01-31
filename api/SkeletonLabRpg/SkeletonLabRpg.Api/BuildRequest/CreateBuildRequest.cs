using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.BuildRequest.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Extensions;
using SkeletonLabRpg.Common.Services.Interfaces;
using SkeletonLabRpg.Common.Services.Models;
using BuildRequestModel = SkeletonLabRpg.Common.Database.Models.Build.BuildRequestModel;

namespace SkeletonLabRpg.Api.BuildRequest;

public static class CreateBuildRequest
{
    public record Request(string Question, Guid BuildSystemId);

    private record BuildRequestResponse(Guid Id, string Question, Guid BuildSystemId, string BuildSystemName, BuildRequestStatus Status);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost(BuildRequestEndpoints.Create, Handler)
                .RequireAuthorization(RoleType.CharacterFullAccess.ToName());
        }

        private static async Task<IResult> Handler(
            [FromBody] Request request,
            [FromServices] AccountDetails accountDetails,
            [FromServices] IBuildRequestPublisher queueSender,
            [FromServices] UserScopedRepository<BuildRequestModel> buildRequestRepository,
            [FromServices] UserScopedRepository<BuildSystemModel> buildSystemRepository)
        {
            var buildSystem = await buildSystemRepository.GetById(request.BuildSystemId);

            if (buildSystem is null)
            {
                return Results.NotFound();
            }
            
            var buildRequest = new BuildRequestModel
            {
                Question = request.Question,
                BuildSystemId = request.BuildSystemId,
                Status = BuildRequestStatus.Processing
            };
            
            var result = await buildRequestRepository.Create(buildRequest);
            
            var queueRequest = new QueueRequest(accountDetails.UserId, buildRequest.Id, buildSystem.Id, request.Question, buildSystem.Name);

            await queueSender.SendQueueAsync(queueRequest);
            
            return Results.Ok(new BuildRequestResponse(
                result.Id,
                result.Question,
                result.BuildSystemId,
                buildSystem.Name,
                result.Status));
        }
    }
}