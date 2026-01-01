using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.BuildSystem.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Exceptions;

namespace SkeletonLabRpg.Api.BuildSystem;

public static class UpdateBuildSystem
{
    private record Request(string Name);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPut(BuildSystemEndpoints.Update, Handler);
        }

        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromBody] Request request,
            [FromServices] AccountDetails accountDetails,
            [FromServices] IRepository<BuildSystemModel> repository,
            [FromServices] ITaskCache<BuildSystemModel> cache)
        {
            var current = await repository.GetById(id);

            if (current is null)
            {
                throw new NotFoundException("Rpg System not found", showCustomMessage: true);
            }

            current.Name = request.Name;
            
            var updated = await repository.Update(current);
            cache.Invalidate(accountDetails.Email, id.ToString());
            return Results.Ok(updated);
        }
    }
}