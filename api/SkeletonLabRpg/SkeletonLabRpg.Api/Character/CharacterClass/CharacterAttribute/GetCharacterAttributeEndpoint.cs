using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models;

namespace SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute;

public static class GetCharacterAttributeEndpoint
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet(CharacterAttributeEndpoints.GetCharacterAttribute, Handler);
        }

        private static async Task<IResult> Handler([FromRoute]Guid characterClassId, [FromServices] IRepository<CharacterAttributes> repository)
        {
            var attribute = await repository.GetByPredicate(attribute => attribute.CharacterClassId == characterClassId);
            return Results.Ok(attribute);
        }
    }
}