using System.Globalization;
using System.Text.Json;
using CsvHelper;
using Microsoft.Azure.Functions.Worker;
using SkeletonLabRpg.Common.AILearning.Models;
using SkeletonLabRpg.Common.Constants;
using SkeletonLabRpg.Common.Database.Models;
using SkeletonLabRpg.Common.Extensions;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.AzureFunctions.Queues;

public class ProcessCharacterAttributes(IBlobStorage blobStorage)
{
    const string QueueName = "characterattributequeue";

    [Function("ProcessCharacterAttributesQueue")]
    public async Task Run(
        [QueueTrigger(QueueName, Connection = "AzureWebJobsStorage")]
        string queueItem)
    {
        var item = JsonSerializer.Deserialize<CharacterAttributes>(queueItem);
        var characterAttributeAi = item.AssignCharacterTagsForAi();

        var blobItem = await blobStorage.DownloadBlobAsync(BlobConstants.CharacterAttributeModelContainer,
            BlobConstants.CharacterAttributeModelFileName);
        if (blobItem is null)
        {
            await using var csvStream = characterAttributeAi.ConvertToCsv();
            await blobStorage.UploadBlobAsync(BlobConstants.CharacterAttributeModelContainer, BlobConstants.CharacterAttributeModelFileName,
                csvStream);
            return;
        }

        using var reader = new StreamReader(blobItem);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var existingRecords = csv.GetRecords<CharacterAttributesAi>().ToList();

        if (characterAttributeAi.DoesItemExistInCharacterAttributesAiList(existingRecords))
        {
            return;
        }

        existingRecords.Add(characterAttributeAi);

        using var memoryStream = new MemoryStream();
        await using var textWriter = new StreamWriter(memoryStream);
        await using var csvHelper = new CsvWriter(textWriter, culture: CultureInfo.InvariantCulture);
        await csvHelper.WriteRecordsAsync(existingRecords);
        await textWriter.FlushAsync();
        memoryStream.Position = 0;
        await blobStorage.UploadBlobAsync(BlobConstants.CharacterAttributeModelContainer, BlobConstants.CharacterAttributeModelFileName,
            memoryStream);
    }
}