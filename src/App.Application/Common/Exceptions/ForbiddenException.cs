using App.Application.Common.Constants;

namespace App.Application.Common.Exceptions;

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message)
        : base(message, 403, ErrorCodes.Forbidden) { }
}
