using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CoEditor.Rest;

internal class LoggingFilter(ILoggerFactory loggerFactory) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller.GetType();
        var logger = loggerFactory.CreateLogger(controller);
        logger.IncomingRequest(context.HttpContext, controller);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var controller = context.Controller.GetType();
        var logger = loggerFactory.CreateLogger(controller);
        if (context.Exception is AuthenticationException)
        {
            logger.UnauthenticatedRequest(context.Exception);
            context.ExceptionHandled = true;
            context.Result = new UnauthorizedResult();
            return;
        }

        if (context.Exception != null)
        {
            logger.FailedRequest(context.Exception);
            context.ExceptionHandled = true;
            context.Result = new StatusCodeResult(500);
            return;
        }

        logger.SuccessfulRequest(context.HttpContext.Response.StatusCode);
    }
}

#pragma warning disable SA1402, SA1204 // LogMessages are only used in this file
internal static partial class LoggingFilterLogMessages
{
    [LoggerMessage(LogLevel.Debug, Message = "Text-Change registered into Undo chain")]
    public static partial void TextChangeRegistered(this ILogger logger);

    public static void IncomingRequest(this ILogger logger, HttpContext httpContext, Type controller)
    {
        logger.LogDebug("Incoming API-Request {Method} {Url} for {Controller}", httpContext.Request.Method, httpContext.Request.Path, controller.Name);
    }

    [LoggerMessage(Level = LogLevel.Warning, EventId = 3001, Message = "Unauthenticated REST request")]
    public static partial void UnauthenticatedRequest(this ILogger logger, Exception e);

    [LoggerMessage(Level = LogLevel.Warning, EventId = 3002, Message = "Failed REST request")]
    public static partial void FailedRequest(this ILogger logger, Exception e);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Request returned successfully with status code {statusCode}")]
    public static partial void SuccessfulRequest(this ILogger logger, int? statusCode);
}
