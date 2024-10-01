
using Vulpes.Zinc.Domain.Logging;

namespace Vulpes.Zinc.Web.Middleware;

public class KnownExceptionHandling : IMiddleware
{
    public static string ExceptionKey => "ExceptionType";
    public static string ExceptionMessageKey => "ExceptionMessage";

    private readonly ILogger<KnownExceptionHandling> logger;

    public KnownExceptionHandling(ILogger<KnownExceptionHandling> logger)
    {
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // TODO: This doesn't seem to work on post.
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            HandleException(context, exception, StatusCodes.Status500InternalServerError);
        }
    }

    private void HandleException(HttpContext context, Exception exception, int status)
    {
        context.Response.StatusCode = status;
        context.Items[ExceptionKey] = exception.GetType().Name;
        context.Items[ExceptionMessageKey] = exception.Message;
        logger.LogError(exception, $"{LogTags.Failure} Failed to execute request to {context.Request.Path}.");
    }
}
