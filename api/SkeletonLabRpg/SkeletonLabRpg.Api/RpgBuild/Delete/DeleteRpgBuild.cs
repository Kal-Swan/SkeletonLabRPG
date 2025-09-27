using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.RpgBuild.Constants;
using SkeletonLabRpg.Api.RpgBuild.Create;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.RpgBuild;

namespace SkeletonLabRpg.Api.RpgBuild.Delete;

public static class DeleteRpgBuild
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapDelete(BuildEndpoints.DeleteRpgBuild, Handler);
        }

        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromServices] IRepository<Build> repository)
        {
            var build = await repository.GetById(id);

            if (build is null)
            {
                return Results.NotFound();
            }
            
            var result = await repository.Delete(id);

            return Results.Ok(result);
        }
    }
}