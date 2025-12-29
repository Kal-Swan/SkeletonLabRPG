using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.BuildRequest.Constants;
using SkeletonLabRpg.Api.BuildRequest.Models;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.BuildRequest;

namespace SkeletonLabRpg.Api.BuildRequest;

public static class UpdateBuildRequest
{
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPut(BuildRequestEndpoints.UpdateBuildRequest,Handler);
        }

        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromRoute] Guid buildAnswerId,
            [FromQuery] BuildAnswerStatus status,
            [FromServices] IRepository<BuildRequestModel> buildRequestRepository,
            [FromServices] IRepository<BuildModel> buildRepository,
            [FromServices] IRepository<BuildSystemModel> buildSystemRepository)
        {
            var buildRequest = await buildRequestRepository.GetById(id);
            if (buildRequest is null)
            {
                return Results.NotFound();
            }

            var buildAnswer = buildRequest.Answers.FirstOrDefault(answer => answer.Id == buildAnswerId);

            buildAnswer!.Status = status;

            if (status == BuildAnswerStatus.Saved)
            {
                var newBuild = new BuildModel
                {
                    Name = buildAnswer.Name,
                    Template = buildAnswer.Template,
                    BuildSystemId = buildRequest.BuildSystemId,
                    AccountEmail = buildRequest.AccountEmail,
                    Reason =  buildAnswer.Reason,
                    BuildRequestId =  buildRequest.Id,
                    Id = buildAnswerId
                };
                await buildRepository.Create(newBuild);
            }

            var updatedModel = await buildRequestRepository.Update(buildRequest);
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