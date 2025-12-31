using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.BuildRequest.Constants;
using SkeletonLabRpg.Api.BuildRequest.Models;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.BuildRequest;

namespace SkeletonLabRpg.Api.BuildRequest;

public static class UpdateBuildRequest
{
    private record Request(Guid Id, string Name, string Reason, string Template, BuildAnswerStatus Status);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPut(BuildRequestEndpoints.UpdateBuildRequest,Handler);
        }

        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromBody] Request request,
            [FromServices] AccountDetails accountDetails,
            [FromServices] IRepository<BuildRequestModel> buildRequestRepository,
            [FromServices] IRepository<BuildModel> buildRepository,
            [FromServices] IRepository<BuildSystemModel> buildSystemRepository,
            [FromServices] ITaskCache<BuildRequestModel> buildRequestCache)
        {
            var buildRequest = await buildRequestRepository.GetById(id);
            if (buildRequest is null)
            {
                return Results.NotFound();
            }

            var buildAnswer = buildRequest.Answers.FirstOrDefault(answer => answer.Id == request.Id);

            buildAnswer!.Status = request.Status;
            buildAnswer.Name = request.Name;
            buildAnswer.Template = request.Template;
            buildAnswer.Reason = request.Reason;

            if (request.Status == BuildAnswerStatus.Saved)
            {
                var newBuild = new BuildModel
                {
                    Name = buildAnswer.Name,
                    Template = buildAnswer.Template,
                    BuildSystemId = buildRequest.BuildSystemId,
                    AccountEmail = buildRequest.AccountEmail,
                    Reason =  buildAnswer.Reason,
                    BuildRequestId =  buildRequest.Id,
                    Id = request.Id
                };
                await buildRepository.Create(newBuild);
            }

            var updatedModel = await buildRequestRepository.Update(buildRequest);
            buildRequestCache.Invalidate(CacheKeys.GetRepositoryGetManyByType<BuildRequestModel>(accountDetails.Email));
            
            var buildSystem = await buildSystemRepository.GetById(updatedModel.BuildSystemId);
            return Results.Ok(new BuildRequestResponse(
                updatedModel.Id,
                updatedModel.Question,
                updatedModel.BuildSystemId,
                buildSystem.Name,
                updatedModel.Status,
                updatedModel.Answers,
                updatedModel.Modified));
        }
    }
}