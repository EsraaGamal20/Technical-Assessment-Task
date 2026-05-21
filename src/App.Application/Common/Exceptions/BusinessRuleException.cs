using App.Application.Common.Constants;

namespace App.Application.Common.Exceptions;

public sealed class BusinessRuleException : AppException
{
    public BusinessRuleException(string message)
        : base(message, 400, ErrorCodes.BusinessRuleViolation) { }

    public BusinessRuleException(string message, string errorCode)
        : base(message, 400, errorCode) { }
}
