using App.Application.Common.Constants;
using App.Application.Common.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace App.Api.Middleware;

public sealed class UnhandledExceptionHandler : IExceptionHandler
{
    private readonly ILogger<UnhandledExceptionHandler> _logger;
    private readonly IHostEnvironment                   _env;

    public UnhandledExceptionHandler(
        ILogger<UnhandledExceptionHandler> logger,
        IHostEnvironment env)
    {
        _logger = logger;
        _env    = env;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && cancellationToken.IsCancellationRequested)
            return true;

        _logger.LogError(exception,
            "Unhandled exception at {Path}: {Message}",
            httpContext.Request.Path, exception.Message);

        var publicMessage = _env.IsDevelopment()
            ? exception.Message
            : "An unexpected error occurred. Please try again later.";

        var errors = _env.IsDevelopment()
            ? new[]
              {
                  new Error("Exception",  exception.GetType().FullName ?? "Unknown"),
                  new Error("StackTrace", exception.StackTrace ?? string.Empty)
              }
            : new[] { new Error(ErrorCodes.InternalServerError, "Internal server error.") };

        var result = Result.Failure(publicMessage, 500, errors);

        httpContext.Response.Clear();
        httpContext.Response.StatusCode  = 500;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
        return true;
    }
}
