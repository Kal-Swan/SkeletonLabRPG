namespace SkeletonLabRpg.Common.Exceptions;

public class MachineLearningException(string Message, bool ShowCustomMessage = false) : Exception(Message);