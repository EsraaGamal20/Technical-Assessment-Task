using App.Application.Common.Constants;
using App.Application.Common.Models;

namespace App.Application.Common.Exceptions;

public sealed class ValidationException : AppException
{
    public ValidationException(IEnumerable<Error> errors)
        : base("One or more validation errors occurred.", 422, ErrorCodes.ValidationFailed, errors) { }

    public ValidationException(string field, string message)
        : base("One or more validation errors occurred.", 422, ErrorCodes.ValidationFailed,
               new[] { new Error(field, message) }) { }
}
