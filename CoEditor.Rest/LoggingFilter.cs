using System.Security.Authentication;
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
