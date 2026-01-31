using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Build.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.Builds;

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
            [FromServices] UserScopedRepository<BuildModel> repository,
            [FromServices] AccountDetails accountDetails)
        {
            var build = new BuildModel
            {
                Name = request.Name,
                BuildSystemId = request.BuildSystemId,
                Template = request.Template,
                Reason = request.Reason
            };
            var result = await repository.Create(build);
            
            return Results.Ok(result);
        }
    }
}