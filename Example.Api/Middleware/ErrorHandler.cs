using Microsoft.AspNetCore.Mvc;
using Example.Api.Exceptions;

namespace Example.Api.Middleware;

public class ErrorHandler
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ErrorHandler> _logger;

    public ErrorHandler(RequestDelegate next, IWebHostEnvironment env, ILogger<ErrorHandler> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            var path = httpContext.Request.Path;
            var query = httpContext.Request.QueryString;
            var method = httpContext.Request.Method;
            var user = httpContext.User?.Identity?.Name ?? "Unknown";
            _logger.LogError(ex, "Exception at {method}, {path}, {query}, user {user}", method, path, query, user);

            string message;
            string? detail;
            if (_env.IsDevelopment())
            {
                message = ex.Message;
                detail = ex.StackTrace;
            }
            else
            {
                message = "Server error";
                detail = null;
                if (ex is AppException)
                    message = ex.Message;
            }

            //RFC 7807
            ProblemDetails problemDetails = new()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = message,
                Detail = detail
            };

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}

public static class ErrorHandlerExtensions
{
    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandler>();
    }
}
