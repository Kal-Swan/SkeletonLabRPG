using System.Globalization;
using CsvHelper;
using SkeletonLabRpg.Common.AILearning.Models;

namespace SkeletonLabRpg.Common.Extensions;

public static class CsvExtensions
{
    public static IEnumerable<CharacterAttributesAi> ConvertFromCsv(this string csv)
    {
        using var streamReader = new StreamReader(csv);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        return csvReader.GetRecords<CharacterAttributesAi>();
    }
}