using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Build.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Api.Build.Delete;

public static class DeleteBuild
{
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapDelete(BuildEndpoints.DeleteRpgBuild, Handler);
        }

        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromServices] IRepository<BuildModel> repository)
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