using App.Application.Common.Constants;

namespace App.Application.Common.Exceptions;

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message)
        : base(message, 401, ErrorCodes.Unauthorized) { }

    public UnauthorizedException(string message, string errorCode)
        : base(message, 401, errorCode) { }
}
