using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Extensions;

namespace SkeletonLabRpg.Api.Character;

public static class DeleteCharacter
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapDelete(CharacterEndpoints.DeleteCharacter, Handler)
                .RequireAuthorization(RoleType.CharacterFullAccess.ToName());
        }
    }
    
    private static async Task<IResult> Handler([FromRoute]Guid id, [FromServices] IRepository<Common.Database.Models.Character> repository)
    {
        var result = await repository.Delete(id);
        
        return !result ? Results.NotFound() : Results.Ok();
    }
}