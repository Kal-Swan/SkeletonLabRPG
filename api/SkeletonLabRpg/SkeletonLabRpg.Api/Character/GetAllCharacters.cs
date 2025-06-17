using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Extensions;

namespace SkeletonLabRpg.Api.Character;

public static class GetAllCharacters
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet(CharacterEndpoints.GetAllCharacters, Handler)
                .RequireAuthorization(RoleType.CharacterFullAccess.ToName());
        }
    }

    private static async Task<IResult> Handler([FromServices]IRepository<Common.Database.Models.Character> repository)
    {
        var characters = await repository.GetMany(character => true);
        return Results.Ok(characters);
    }
}