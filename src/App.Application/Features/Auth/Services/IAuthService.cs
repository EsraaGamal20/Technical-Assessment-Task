using App.Application.Common.Models;
using App.Application.Features.Auth.Dtos;

namespace App.Application.Features.Auth.Services;

public interface IAuthService
{
    Task<Result<RegisterInitiateResponse>> RegisterInitiateAsync(RegisterInitiateRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken ct = default);
    Task<Result<RegisterInitiateResponse>> ResendOtpAsync(ResendOtpRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
