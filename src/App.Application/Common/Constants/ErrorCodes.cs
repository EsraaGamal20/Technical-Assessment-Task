namespace App.Application.Common.Constants;

public static class ErrorCodes
{
    public const string NotFound              = "NOT_FOUND";
    public const string ValidationFailed      = "VALIDATION_FAILED";
    public const string Conflict              = "CONFLICT";
    public const string Unauthorized          = "UNAUTHORIZED";
    public const string Forbidden             = "FORBIDDEN";
    public const string BusinessRuleViolation = "BUSINESS_RULE_VIOLATION";
    public const string InvalidCredentials    = "INVALID_CREDENTIALS";
    public const string OtpInvalid            = "OTP_INVALID";
    public const string OtpExpired            = "OTP_EXPIRED";
    public const string OtpMaxAttempts        = "OTP_MAX_ATTEMPTS";
    public const string OtpResendCooldown     = "OTP_RESEND_COOLDOWN";
    public const string PhoneAlreadyUsed      = "PHONE_ALREADY_USED";
    public const string EmailAlreadyUsed      = "EMAIL_ALREADY_USED";
    public const string InternalServerError   = "INTERNAL_SERVER_ERROR";
}
