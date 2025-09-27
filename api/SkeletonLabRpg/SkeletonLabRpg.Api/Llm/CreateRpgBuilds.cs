using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.Llm.Constants;
using SkeletonLabRpg.Api.Llm.External;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Extensions;

namespace SkeletonLabRpg.Api.Llm;

public static class CreateRpgBuilds
{
    public record LlmQuestionRequest(string Question, string RpgSystem, string Template);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost(LlmEndpoints.CreateRpgBuilds, Handler)
                .RequireAuthorization(RoleType.CharacterFullAccess.ToName());
        }

        private static async Task<IResult> Handler(
            [FromBody] LlmQuestionRequest request,
            [FromServices] LlmService llmService)
        {
            var result = await llmService.GetBuildsByQuestion(request.Question, request.RpgSystem);
            return Results.Ok(result);
        }
    }
}