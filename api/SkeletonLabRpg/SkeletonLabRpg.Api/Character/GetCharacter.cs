using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Extensions;

namespace SkeletonLabRpg.Api.Character;

public static class GetCharacter
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet(CharacterEndpoints.GetCharacter, Handler)
                .RequireAuthorization(RoleType.CharacterFullAccess.ToName());
        }
    }

    private static async Task<IResult> Handler([FromRoute]Guid id, [FromServices]IRepository<Common.Database.Models.Character> repository)
    {
        var character = await repository.GetById(id);
        return Results.Ok(character);
    }
}