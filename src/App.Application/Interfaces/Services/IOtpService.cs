namespace App.Application.Interfaces.Services;

public interface IOtpService
{
    Task<string> GenerateAndStoreAsync(string phoneNumber, CancellationToken ct = default);
    Task<bool> ValidateAsync(string phoneNumber, string code, CancellationToken ct = default);
    Task InvalidateAsync(string phoneNumber, CancellationToken ct = default);
    Task<bool> CanResendAsync(string phoneNumber, CancellationToken ct = default);
}
