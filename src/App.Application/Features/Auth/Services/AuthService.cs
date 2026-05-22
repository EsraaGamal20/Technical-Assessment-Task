using App.Application.Common.Constants;
using App.Application.Common.Exceptions;
using App.Application.Common.Models;
using App.Application.Features.Auth.Dtos;
using App.Application.Interfaces.Persistence;
using App.Application.Interfaces.Services;
using App.Application.Mappings;
using App.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace App.Application.Features.Auth.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IOtpService _otp;
    private readonly IJwtTokenService _jwt;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<AuthService> _logger;

    private const int OtpExpiresInSeconds = 5 * 60;

    public AuthService(
        IUnitOfWork uow,
        IPasswordHasher hasher,
        IOtpService otp,
        IJwtTokenService jwt,
        IDateTimeProvider clock,
        ILogger<AuthService> logger)
    {
        _uow = uow;
        _hasher = hasher;
        _otp = otp;
        _jwt = jwt;
        _clock = clock;
        _logger= logger;
    }

    public async Task<Result<RegisterInitiateResponse>> RegisterInitiateAsync(
        RegisterInitiateRequest request, CancellationToken ct = default)
    {
        if (await _uow.Users.EmailExistsAsync(request.Email, ct))
            throw new ConflictException("Email is already registered.", ErrorCodes.EmailAlreadyUsed);

        if (await _uow.Users.PhoneExistsAsync(request.PhoneNumber, ct))
            throw new ConflictException("Phone number is already registered.", ErrorCodes.PhoneAlreadyUsed);

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var user = new ApplicationUser
            {
                Id=Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber  = request.PhoneNumber,
                PasswordHash = _hasher.Hash(request.Password)
            };

            await _uow.Users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            await _otp.GenerateAndStoreAsync(request.PhoneNumber, ct);

            await _uow.CommitTransactionAsync(ct);

            _logger.LogInformation("Registration initiated for {Phone}", request.PhoneNumber);

            return Result<RegisterInitiateResponse>.Success(new RegisterInitiateResponse(
                request.PhoneNumber,
                OtpExpiresInSeconds,
                "OTP sent to your phone number. Please verify to complete registration."));
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<Result<AuthResponse>> VerifyOtpAsync(
        VerifyOtpRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByPhoneAsync(request.PhoneNumber, ct)
            ?? throw new NotFoundException("User", request.PhoneNumber);

        var valid = await _otp.ValidateAsync(request.PhoneNumber, request.Code, ct);
        if (!valid)
            throw new UnauthorizedException("Invalid or expired OTP.", ErrorCodes.OtpInvalid);

        user.IsPhoneVerified = true;
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync(ct);

        await _otp.InvalidateAsync(request.PhoneNumber, ct);

        return Result<AuthResponse>.Success(BuildAuthResponse(user));
    }

    public async Task<Result<RegisterInitiateResponse>> ResendOtpAsync(
        ResendOtpRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByPhoneAsync(request.PhoneNumber, ct)
            ?? throw new NotFoundException("User", request.PhoneNumber);

        if (user.IsPhoneVerified)
            throw new ConflictException("Phone number is already verified.", ErrorCodes.Conflict);

        var canResend = await _otp.CanResendAsync(request.PhoneNumber, ct);
        if (!canResend)
            throw new BusinessRuleException("Please wait before requesting a new OTP.", ErrorCodes.OtpResendCooldown);

        await _otp.GenerateAndStoreAsync(request.PhoneNumber, ct);

        return Result<RegisterInitiateResponse>.Success(new RegisterInitiateResponse(
            request.PhoneNumber,
            OtpExpiresInSeconds,
            "OTP resent to your phone number."));
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByEmailAsync(request.Email, ct);

        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.", ErrorCodes.InvalidCredentials);

        if (!user.IsPhoneVerified)
            throw new UnauthorizedException("Phone number not verified.", ErrorCodes.Unauthorized);

        return Result<AuthResponse>.Success(BuildAuthResponse(user));
    }

    private AuthResponse BuildAuthResponse(ApplicationUser user)
    {
        var accessToken = _jwt.GenerateAccessToken(user);
        var expiresAt   = _jwt.GetAccessTokenExpiration();
        var expiresIn   = (int)(expiresAt - _clock.UtcNow).TotalSeconds;

        return new AuthResponse(accessToken, "Bearer", expiresAt, expiresIn, user.ToDto());
    }
}
