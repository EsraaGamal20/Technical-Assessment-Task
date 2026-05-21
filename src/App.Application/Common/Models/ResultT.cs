namespace App.Application.Common.Models;

public sealed class Result<T> : Result
{
    public T? Data { get; }

    private Result(T? data, bool isSuccess, string? message, int statusCode, IEnumerable<Error>? errors)
        : base(isSuccess, message, statusCode, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string? message = null, int statusCode = 200)
        => new(data, true, message, statusCode, null);

    public new static Result<T> Failure(string message, int statusCode = 400, IEnumerable<Error>? errors = null)
        => new(default, false, message, statusCode, errors);

    public new static Result<T> Failure(IEnumerable<Error> errors, int statusCode = 400)
        => new(default, false, null, statusCode, errors);

    public new static Result<T> NotFound(string message)
        => new(default, false, message, 404, null);

    public new static Result<T> Conflict(string message)
        => new(default, false, message, 409, null);

    public new static Result<T> Unauthorized(string message)
        => new(default, false, message, 401, null);

    public new static Result<T> Forbidden(string message)
        => new(default, false, message, 403, null);

    public new static Result<T> ValidationFailure(IEnumerable<Error> errors)
        => new(default, false, "One or more validation errors occurred.", 422, errors);

    public static implicit operator Result<T>(T value) => Success(value);
}
