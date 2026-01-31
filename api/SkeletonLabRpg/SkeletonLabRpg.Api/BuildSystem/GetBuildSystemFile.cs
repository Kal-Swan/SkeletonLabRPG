using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.BuildSystem.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Constants;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Api.BuildSystem;

public static class GetBuildSystemFile
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet(BuildSystemEndpoints.File, Handler);
        }

        private static async Task<IResult> Handler(
            Guid id,
            string fileName,
            [FromServices] IBlobStorage blobStorage,
            [FromServices] AccountDetails accountDetails)
        {
            var blobItem = await blobStorage.DownloadBlobAsync(BlobStorageConstants.UserBuildSystemContainer, $"{accountDetails.UserId}/{id}/{fileName}");
            return Results.File(blobItem.Value.Content, blobItem.Value.ContentType, fileName);
        }
    }
}