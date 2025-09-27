using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.RpgBuild.Constants;
using SkeletonLabRpg.Api.RpgBuild.Create;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.RpgBuild;

namespace SkeletonLabRpg.Api.RpgBuild.Update;

public static class UpdateRpgBuild
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPut(BuildEndpoints.UpdateRpgBuild, Handler);
        }

        private static async Task<IResult> Handler(
            [FromBody] BuildRequest request,
            [FromRoute] Guid id,
            [FromServices] IRepository<Build> repository)
        {
            var build = await repository.GetById(id);
            
            if (build is null)
            {
                return Results.NotFound();
            }

            build.Name = request.Name;
            build.Template = request.Template;
            
            var result = await repository.Update(build);
            
            return Results.Ok(result);
        }
    }
}