namespace App.Application.Features.Auth.Dtos;

public sealed record RegisterInitiateResponse(
    string PhoneNumber,
    int    OtpExpiresInSeconds,
    string Message);
