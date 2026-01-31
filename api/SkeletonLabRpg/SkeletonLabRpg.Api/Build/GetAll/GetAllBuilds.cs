using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Build.Constants;
using SkeletonLabRpg.Api.Build.Extensions;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.Builds;

namespace SkeletonLabRpg.Api.Build.GetAll;

public static class GetAllBuilds
{
    private record Response(Dictionary<Guid, List<BuildResponse>> GroupBuilds, IEnumerable<BuildSystemResponse> BuildSystems);

    private record BuildSystemResponse(Guid Id, string Name);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet(BuildEndpoints.GetAllRpgBuilds, Handler);
        }

        private static async Task<IResult> Handler(
            [FromServices] AccountDetails accountDetails,
            [FromServices] UserScopedRepository<BuildModel> buildRepository,
            [FromServices] UserScopedRepository<BuildSystemModel> buildSystemRepository)
        {
            var builds = await buildRepository.GetAll();
            var systems = await buildSystemRepository.GetAll();
            
            var groupBuilds = builds
                .Select(build => build.ToBuildResponseDto())
                .GroupBy(build => build.BuildSystemId)
                .ToDictionary(group => group.Key, group => group.ToList());

            return Results.Ok(new Response(groupBuilds, systems.Select(system => new  BuildSystemResponse(system.Id, system.Name))));
        }
    }
}