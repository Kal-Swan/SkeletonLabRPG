using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.Build.Constants;
using SkeletonLabRpg.Api.Build.Extensions;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.Build;

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
            [FromServices] IRepository<BuildModel> buildRepository,
            [FromServices] IRepository<BuildSystemModel> buildSystemRepository)
        {
            var builds = await buildRepository.GetMany(build => build.AccountEmail == accountDetails.Email, accountDetails.Email);
            var systems = await buildSystemRepository.GetMany(system => system.AccountEmail == accountDetails.Email, accountDetails.Email);
            
            var groupBuilds = builds
                .Select(build => build.ToBuildResponseDto())
                .GroupBy(build => build.BuildSystemId)
                .ToDictionary(group => group.Key, group => group.ToList());

            return Results.Ok(new Response(groupBuilds, systems.Select(system => new  BuildSystemResponse(system.Id, system.Name))));
        }
    }
}