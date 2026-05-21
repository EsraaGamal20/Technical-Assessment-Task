using App.Application.Common.Constants;

namespace App.Application.Common.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, 404, ErrorCodes.NotFound) { }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.", 404, ErrorCodes.NotFound) { }
}
