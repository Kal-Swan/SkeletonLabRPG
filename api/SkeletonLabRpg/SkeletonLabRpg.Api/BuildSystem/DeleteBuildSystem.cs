using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.BuildSystem.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Exceptions;

namespace SkeletonLabRpg.Api.BuildSystem;

public static class DeleteBuildSystem
{
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapDelete(BuildSystemEndpoints.Delete, Handler);
        }
        
        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromServices] IRepository<BuildSystemModel> repository)
        {
            var current = await repository.GetById(id);

            if (current is null)
            {
                throw new NotFoundException("Rpg System already deleted", showCustomMessage: true);
            }
            
            var deleted = await repository.Delete(id);

            return Results.Ok(deleted);
        }
    }
}