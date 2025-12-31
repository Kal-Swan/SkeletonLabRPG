using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.BuildSystem.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Exceptions;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Api.BuildSystem;

public static class CreateBuildSystem
{

    public record Request(string Name);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost(BuildSystemEndpoints.Base, Handler);
        }

        private static async Task<IResult> Handler(
            [FromBody] Request request,
            [FromServices] AccountDetails accountDetails,
            [FromServices] IRepository<BuildSystemModel> repository,
            [FromServices] ITaskCache<BuildSystemModel> cache)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new BadRequestException("Rpg System Name is required", showCustomMessage: true);
            }
            
            var rpgSystem = new BuildSystemModel
            {
                Name = request.Name,
                AccountEmail = accountDetails.Email
            };
            
            var result = await repository.Create(rpgSystem);
            
            cache.Invalidate(CacheKeys.GetRepositoryGetManyByType<BuildSystemModel>(accountDetails.Email));
            
            return Results.Ok(result);
        }
    } 
}