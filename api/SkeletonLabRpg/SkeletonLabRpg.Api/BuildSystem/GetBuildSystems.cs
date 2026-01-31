using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.BuildSystem.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Cosmosdb.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Api.BuildSystem;

public static class GetBuildSystems
{
    private record Response(Guid Id, string Name, IEnumerable<string> FileNames);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet(BuildSystemEndpoints.Base, Handler);
        }

        private static async Task<IResult> Handler(
            [FromServices] AccountDetails accountDetails,
            [FromServices] UserScopedRepository<BuildSystemModel> repository)
        {
            var systems = await repository.GetManyByPredicate(system => 
                system.Status != DocumentStatus.Deleting
                && system.Status != DocumentStatus.Deleted);
            
            return Results.Ok(systems.Select(system =>
                new Response(system.Id, system.Name, system.FileNames)));
        }
    }
}