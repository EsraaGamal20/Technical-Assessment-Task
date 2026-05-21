using App.Application.Common.Constants;

namespace App.Application.Common.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, 409, ErrorCodes.Conflict) { }

    public ConflictException(string message, string errorCode)
        : base(message, 409, errorCode) { }
}
