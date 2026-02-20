using Azure.Storage.Blobs;

namespace SkeletonLabRpg.Common.Services.Interfaces;

public interface IBlobStorage
{
    Task UploadBlobAsync(string containerName, Guid systemId, string blobName, Stream data, string contentType);

    Task<(Stream Content, string ContentType)?> DownloadBlobAsync(string containerName, Guid systemId, string blobName);

    BlobClient GetBlobClient(string containerName, Guid systemId, string blobName);

    Task DeleteBlobAsync(string containerName, Guid systemId, string blobName);
}