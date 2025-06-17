using DEXRPG.WebApi.Authorisation;
using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Character.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Exceptions;
using SkeletonLabRpg.Common.Extensions;

namespace SkeletonLabRpg.Api.Character;

public static class CreateCharacter
{
    public record Request(string Name);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost(CharacterEndpoints.CreateCharacter, Handler)
                .RequireAuthorization(RoleType.CharacterFullAccess.ToName());
        }
    }

    private static async Task<IResult> Handler(
        [FromBody]Request request, 
        [FromServices]IRepository<Common.Database.Models.Character> repository,
        [FromServices] AccountDetails accountDetails,
        [FromServices] ILogger<Endpoint> logger)
    {
        var errors = Validate(request);

        if (errors.Any())
        {
            logger.LogWarning("Validation failed for CreateCharacter request: {Errors}", string.Join(", ", errors));
        }
        //throw new BadRequestException($"Validation failed with errors {string.Join(", ", errors)}");

        var character = new Common.Database.Models.Character
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            AccountEmail = accountDetails.Email
        };
        
        var result = await repository.Create(character);
        logger.LogInformation("Character created with id {Id} and name {Name}", character.Id, character.Name);
        return Results.Ok(result);
    }

    private static IEnumerable<string> Validate(Request request)
    {
        if (request.Name == string.Empty)
        {
            yield return "Name is required";
        }
        
        if (request.Name.Length < 1)
        {
            yield return "Name should be greater than 1 character";
        }

        if (request.Name.Length > 100)
        {
            yield return "Name is too long";
        }
    }
}