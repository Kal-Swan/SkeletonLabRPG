using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Extensions;

namespace SkeletonLabRpg.Api.Character;

public static class DeleteAllCharacters
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete(CharacterEndpoints.DeleteAllCharacters, Handler)
                .RequireAuthorization(RoleType.CharacterFullAccess.ToName());
        }
    }

    private static async Task<IResult> Handler([FromServices]IRepository<Common.Database.Models.Character> repository)
    {
        var characters = await repository.GetMany(character => true);
        var characterIds = characters.Select(x => x.Id).ToList();
        await repository.DeleteMany(character => characterIds.Contains(character.Id));
        return Results.Ok();
    }
}