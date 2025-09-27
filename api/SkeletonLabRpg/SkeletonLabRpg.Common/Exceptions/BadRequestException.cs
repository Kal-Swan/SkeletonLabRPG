namespace SkeletonLabRpg.Common.Exceptions;

public class BadRequestException : Exception
{
    public string Message { get; set;  } 
    public bool ShowCustomMessage { get; set; }

    public BadRequestException(string message, bool showCustomMessage = false) : base(message)
    {
        Message = message;
        ShowCustomMessage = showCustomMessage;
    }
}