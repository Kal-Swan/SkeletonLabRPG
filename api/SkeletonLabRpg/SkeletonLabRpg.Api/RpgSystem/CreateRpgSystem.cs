using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.RpgSystem.Constants;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.RpgBuild;
using SkeletonLabRpg.Common.Exceptions;

namespace SkeletonLabRpg.Api.RpgSystem;

public static class CreateRpgSystem
{

    public record Request(string Name);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost(RpgSystemEndpoints.Base, Handler);
        }

        private async Task<IResult> Handler(
            [FromBody] Request request,
            [FromServices] AccountDetails accountDetails,
            [FromServices] IRepository<RpgSystemModel> repository)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BadRequestException("Rpg System Name is required", showCustomMessage: true);
            }
            
            var rpgSystem = new RpgSystemModel
            {
                Name = request.Name,
                AccountEmail = accountDetails.Email
            };
            
            var result = await repository.Create(rpgSystem);
            
            return Results.Ok(result);
        }
    } 
}