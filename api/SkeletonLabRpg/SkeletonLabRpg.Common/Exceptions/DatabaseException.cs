namespace SkeletonLabRpg.Common.Exceptions;

public class DatabaseException(string Message, bool ShowCustomMessage = false) : Exception(Message);