using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.RpgSystem.Constants;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.RpgBuild;
using SkeletonLabRpg.Common.Exceptions;

namespace SkeletonLabRpg.Api.RpgSystem;

public static class UpdateRpgSystem
{
    private record Request(string Name);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPut(RpgSystemEndpoints.Update, Handler);
        }

        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromBody] Request request,
            [FromServices] IRepository<RpgSystemModel> repository)
        {
            var current = await repository.GetById(id);

            if (current is null)
            {
                throw new NotFoundException("Rpg System not found", showCustomMessage: true);
            }

            current.Name = request.Name;
            
            var updated = await repository.Update(current);

            return Results.Ok(updated);
        }
    }
}