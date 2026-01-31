using Azure.Storage.Blobs;

namespace SkeletonLabRpg.Common.Services.Interfaces;

public interface IBlobStorage
{
    Task UploadBlobAsync(string containerName, string blobName, Stream data, string contentType);

    Task<(Stream Content, string ContentType)?> DownloadBlobAsync(string containerName, string blobName);

    BlobClient GetBlobClient(string containerName, string blobName);

    Task DeleteBlobAsync(string containerName, string blobName);
}