namespace App.Infrastructure.Services.Otp;

internal sealed record OtpRecord(
    string Code,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc,
    int Attempts);
