using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Extensions;

namespace SkeletonLabRpg.Api.Character;

public static class UpdateCharacterName
{
    public record Request(string Name);
    
    public record Response(Guid Id, string Name);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPut(CharacterEndpoints.UpdateCharacterName, Handler)
                .RequireAuthorization(RoleType.CharacterFullAccess.ToName());
        }
    }

    private static async Task<IResult> Handler([FromRoute] Guid id, [FromBody]Request request, [FromServices] IRepository<Common.Database.Models.Character> repository)
    {
        var character = await repository.GetById(id);
        if (character == null)
        {
            return Results.NotFound();
        }
            
        character.Name = request.Name;
        var result = await repository.Update(character);
        return Results.Ok(new Response(result.Id, result.Name));
    }
}