using System.Net;

namespace SkeletonLabRpg.Api.Exceptions;

public record ApiErrorResponse(HttpStatusCode StatusCode, ExceptionType Type, string Message);