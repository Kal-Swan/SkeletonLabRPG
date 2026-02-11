using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.BuildRequest.Constants;
using SkeletonLabRpg.Api.BuildRequest.Models;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Api.BuildRequest;

public static class GetDetails
{
    private record BuildSystemResponse(Guid Id, string Name);
    
    private record Response(IEnumerable<BuildRequestResponse> BuildRequests, IEnumerable<BuildSystemResponse> BuildSystems);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet(BuildRequestEndpoints.GetBuildRequestDetails, Handler);
        }
        
        private static async Task<IResult> Handler(
            [FromServices] AccountDetails accountDetails,
            [FromServices] UserScopedRepository<BuildRequestModel> buildRequestRepository,
            [FromServices] UserScopedRepository<BuildSystemModel> buildSystemRepository,
            [FromServices] ILogger<IEndpoint> logger)
        {
            var buildSystems = await buildSystemRepository.GetAll();
            var requests = await buildRequestRepository.GetManyByPredicate(
                request => request.Answers.Any(answer => answer.Status == BuildAnswerStatus.None) || !request.Answers.Any());
            
            var buildSystemDictionary = buildSystems.ToDictionary(system => system.Id, model => model.Name);
            
            var responseRequests = requests
                .Where(request => buildSystemDictionary.ContainsKey(request.BuildSystemId))
                .Select(request => 
                    new BuildRequestResponse(
                        request.Id, 
                        request.Question, 
                        request.BuildSystemId, 
                        buildSystemDictionary[request.BuildSystemId], 
                        request.Status,
                        request.Answers,
                        request.Modified,
                        request.Progression));
                        

            if (requests.Count() != responseRequests.Count())
            {
                logger.LogError("Some build requests were skipped because their build system was not found. AccountEmail: {AccountEmail}", accountDetails.Email);
            }
            
            return Results.Ok(new Response(
                responseRequests, 
                buildSystems.Select(system => new BuildSystemResponse(system.Id, system.Name))
                ));
        }
    }
}