using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoEditor.Rest;

// TODO: General: remove LoggerMessage for direct calls?
#pragma warning disable SA1202 // Access musst be checked afterwards
internal static partial class LogMessages
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Incoming API-Request {method} {url} for {controller}")]
    private static partial void IncomingRequest(this ILogger logger, string method, string url, string controller);

    public static void IncomingRequest(this ILogger logger, HttpContext httpContext, Type controller)
    {
        logger.IncomingRequest(httpContext.Request.Method, httpContext.Request.Path, controller.Name);
    }

    [LoggerMessage(Level = LogLevel.Warning, EventId = 3001, Message = "Unauthenticated REST request")]
    public static partial void UnauthenticatedRequest(this ILogger logger, Exception e);

    [LoggerMessage(Level = LogLevel.Warning, EventId = 3002, Message = "Failed REST request")]
    public static partial void FailedRequest(this ILogger logger, Exception e);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Request returned successfully with status code {statusCode}")]
    public static partial void SuccessfulRequest(this ILogger logger, int? statusCode);
}
