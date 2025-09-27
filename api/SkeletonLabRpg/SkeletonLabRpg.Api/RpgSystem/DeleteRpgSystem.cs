using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.RpgSystem.Constants;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.RpgBuild;
using SkeletonLabRpg.Common.Exceptions;

namespace SkeletonLabRpg.Api.RpgSystem;

public static class DeleteRpgSystem
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapDelete(RpgSystemEndpoints.Delete, Handler);
        }
        
        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromServices] IRepository<RpgSystemModel> repository)
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