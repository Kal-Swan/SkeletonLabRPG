using DEXRPG.WebApi.Authorisation;
using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.CharacterClass.Constants;
using SkeletonLabRpg.Api.Character.CharacterClass.Models;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;

namespace SkeletonLabRpg.Api.Character.CharacterClass;

public static class CreateCharacterClassEndpoint
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost(CharacterClassEndpoints.CreateCharacterClass,Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] Guid characterId,
            [FromBody] CharacterClassRequest request,
            IRepository<Common.Database.Models.CharacterClass> repository,
            AccountDetails accountDetails)
        {
            var model = new Common.Database.Models.CharacterClass
            {
                Name = request.Name,
                ClassType = request.ClassType,
                CharacterId = characterId,
                AccountEmail = accountDetails.Email
            };
            
            await repository.Create(model);

            return Results.Created();
        }
    }
}