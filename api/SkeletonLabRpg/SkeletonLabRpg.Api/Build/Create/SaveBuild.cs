using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.Build.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Api.Build.Create;

public static class SaveBuild
{
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost(BuildEndpoints.CreateRpgBuild, Handler);
        }

        private static async Task<IResult> Handler(
            [FromBody] BuildRequest request,
            [FromServices] IRepository<BuildModel> repository,
            [FromServices] AccountDetails accountDetails)
        {
            var build = new BuildModel
            {
                Name = request.Name,
                BuildSystemId = request.BuildSystemId,
                Template = request.Template,
                Reason = request.Reason,
                AccountEmail = accountDetails.Email,
            };
            var result = await repository.Create(build);
            
            return Results.Ok(result);
        }
    }
}