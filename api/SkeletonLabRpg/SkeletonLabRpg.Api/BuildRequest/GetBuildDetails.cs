using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.BuildRequest.Constants;
using SkeletonLabRpg.Api.BuildRequest.Models;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.BuildRequest;

namespace SkeletonLabRpg.Api.BuildRequest;

public static class GetBuildDetails
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
            [FromServices] IRepository<BuildRequestModel> buildRequestRepository,
            [FromServices] IRepository<BuildSystemModel> buildSystemRepository,
            [FromServices] ILogger<IEndpoint> logger)
        {
            var buildSystems = await buildSystemRepository.GetMany(request => 
                request.AccountEmail == accountDetails.Email, accountDetails.Email);
            var requests = await buildRequestRepository.GetMany(
                request => request.AccountEmail == accountDetails.Email && (request.Answers.Any(answer => answer.Status == BuildAnswerStatus.None) 
                                                                            || !request.Answers.Any()),
                accountDetails.Email);
            
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
                        request.Modified));
                        

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