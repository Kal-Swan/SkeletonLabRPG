using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SkeletonLabRpg.Api.BuildRequest.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.Filters;
using SkeletonLabRpg.Api.SignalR;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.User;

namespace SkeletonLabRpg.Api.BuildRequest;

public static class GetProgress
{
    private record Response(Guid Id, BuildRequestStatus Status, int Progression);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet(BuildRequestEndpoints.Progression, Handler)
                .WithMetadata(new WorkerEndpointMetadata())
                .AddEndpointFilter<WorkerKeyFilter>();
        }

        private static async Task Handler(
            [FromRoute] Guid id,
            [FromQuery] int percent,
            [FromServices] AccountDetails accountDetails,
            [FromServices] IRepository<UserAccount> userAccountRepository,
            [FromServices] UserScopedRepository<BuildRequestModel> buildRequestRepository,
            [FromServices] IHubContext<BuildHub> buildHubContext)
        {
            try
            {
                await buildHubContext.Clients.User(accountDetails.AzureIdentityObjectId)
                    .SendAsync(
                        SignalRHubMethodConstants.BuildRequestProgress, 
                        new Response(id, BuildRequestStatus.Processing, percent));
            }
            finally
            {
                var buildRequest = await buildRequestRepository.GetById(id);
                buildRequest.Status = BuildRequestStatus.Processing;
                buildRequest.Progression = percent;
                await buildRequestRepository.Update(buildRequest);
            }
        }
    }
}