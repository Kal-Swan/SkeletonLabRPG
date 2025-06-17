using DEXRPG.WebApi.Authorisation;
using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute.Constants;
using SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute.Extensions;
using SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute.Models;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute;

public static class CreateCharacterAttributeEndpoint
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost(CharacterAttributeEndpoints.CreateCharacterAttribute, Handler);
        }

        private static async Task<IResult> Handler(
            [FromBody]CreateCharacterAttributeRequest request,
            [FromRoute] Guid characterClassId,
            [FromServices] IRepository<CharacterAttributes> repository,
            [FromServices] IStorageQueue<CharacterAttributeRequest> storageQueue,
            AccountDetails accountDetails)
        {
            await repository.Create(request.Favourite.ToCharacterAttributesModel(accountDetails.Email, characterClassId));
            await storageQueue.SendMessageAsync(request.Others.Append(request.Favourite));
            return Results.Ok();    
        }
    }
}