using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.RpgBuild.Constants;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.RpgBuild;

namespace SkeletonLabRpg.Api.RpgBuild.Create;

public static class SaveRpgBuild
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost(BuildEndpoints.CreateRpgBuild, Handler);
        }

        private static async Task<IResult> Handler(
            [FromBody] BuildRequest request,
            [FromServices] IRepository<Build> repository,
            [FromServices] AccountDetails accountDetails)
        {
            var build = new Build
            {
                Name = request.Name,
                RpgSystemId = request.RpgSystemId,
                Template = request.Template,
                Reason = request.Reason,
                AccountEmail = accountDetails.Email,
            };
            var result = await repository.Create(build);
            
            return Results.Ok(result);
        }
    }
}