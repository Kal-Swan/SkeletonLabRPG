namespace SkeletonLabRpg.Common.Constants;

public static class FileValidation
{
    public const string Pdf = "application/pdf";
    public const string PlainText = "text/plain";
    
    public static IReadOnlyList<string> AllowedFileTypes =
    [
        Pdf,
        PlainText
    ];
}