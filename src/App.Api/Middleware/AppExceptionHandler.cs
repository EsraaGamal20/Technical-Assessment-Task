using App.Application.Common.Exceptions;
using App.Application.Common.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace App.Api.Middleware;

public sealed class AppExceptionHandler : IExceptionHandler
{
    private readonly ILogger<AppExceptionHandler> _logger;

    public AppExceptionHandler(ILogger<AppExceptionHandler> logger)
        => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not AppException appEx)
            return false;

        var level = appEx.StatusCode >= 500 ? LogLevel.Error : LogLevel.Information;
        _logger.Log(level, appEx,
            "Handled {ExceptionType} ({ErrorCode}) at {Path}: {Message}",
            appEx.GetType().Name, appEx.ErrorCode,
            httpContext.Request.Path, appEx.Message);

        var errors = appEx.Errors.Count > 0
            ? appEx.Errors
            : new[] { new Error(appEx.ErrorCode, appEx.Message) };

        var result = appEx.StatusCode switch
        {
            422 => Result.ValidationFailure(errors),
            _   => Result.Failure(appEx.Message, appEx.StatusCode, errors)
        };

        httpContext.Response.Clear();
        httpContext.Response.StatusCode  = result.StatusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
        return true;
    }
}
