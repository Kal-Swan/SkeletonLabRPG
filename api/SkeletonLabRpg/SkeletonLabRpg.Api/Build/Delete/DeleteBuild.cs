using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Build.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Cosmosdb.Constants;
using SkeletonLabRpg.Common.Database.Cosmosdb.Helper;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.Builds;

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
            [FromServices] UserScopedRepository<BuildModel> repository,
            [FromServices] AccountDetails accountDetails)
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