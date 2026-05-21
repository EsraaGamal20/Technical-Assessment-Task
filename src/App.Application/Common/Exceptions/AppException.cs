using App.Application.Common.Models;

namespace App.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }
    public IReadOnlyList<Error> Errors { get; }

    protected AppException(string message, int statusCode, string errorCode, IEnumerable<Error>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode  = errorCode;
        Errors     = errors?.ToList() ?? new List<Error>();
    }
}
