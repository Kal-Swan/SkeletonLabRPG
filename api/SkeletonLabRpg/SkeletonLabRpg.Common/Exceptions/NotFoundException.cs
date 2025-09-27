namespace SkeletonLabRpg.Common.Exceptions;

public class NotFoundException : Exception
{
    public string Message { get; set;  } 
    public bool ShowCustomMessage { get; set; }

    public NotFoundException(string message, bool showCustomMessage = false) : base(message)
    {
        Message = message;
        ShowCustomMessage = showCustomMessage;
    }
}