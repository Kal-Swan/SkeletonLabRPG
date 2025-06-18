using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.CharacterClass.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;

namespace SkeletonLabRpg.Api.Character.CharacterClass;

public static class GetCharacterClassesEndpoint
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet(CharacterClassEndpoints.GetCharacterClasses, Handle);
        }

        private static async Task<IResult> Handle([FromRoute] Guid characterId, [FromServices] IRepository<Common.Database.Models.CharacterClass> repository)
        {
            var classes = await repository.GetMany(@class => @class.CharacterId == characterId);
            return Results.Ok(classes);
        }
    }
}