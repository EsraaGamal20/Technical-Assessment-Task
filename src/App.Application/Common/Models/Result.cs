namespace App.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Message { get; }
    public IReadOnlyList<Error> Errors { get; }
    public int StatusCode { get; }

    protected Result(bool isSuccess, string? message, int statusCode, IEnumerable<Error>? errors)
    {
        IsSuccess  = isSuccess;
        Message    = message;
        StatusCode = statusCode;
        Errors     = errors?.ToList() ?? new List<Error>();
    }

    public static Result Success(string? message = null, int statusCode = 200)
        => new(true, message, statusCode, null);

    public static Result Failure(string message, int statusCode = 400, IEnumerable<Error>? errors = null)
        => new(false, message, statusCode, errors);

    public static Result Failure(IEnumerable<Error> errors, int statusCode = 400)
        => new(false, null, statusCode, errors);

    public static Result NotFound(string message)
        => new(false, message, 404, null);

    public static Result Conflict(string message)
        => new(false, message, 409, null);

    public static Result Unauthorized(string message)
        => new(false, message, 401, null);

    public static Result Forbidden(string message)
        => new(false, message, 403, null);

    public static Result ValidationFailure(IEnumerable<Error> errors)
        => new(false, "One or more validation errors occurred.", 422, errors);
}
