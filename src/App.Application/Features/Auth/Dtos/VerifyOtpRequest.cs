namespace App.Application.Features.Auth.Dtos;

public sealed record VerifyOtpRequest(
    string PhoneNumber,
    string Code);
