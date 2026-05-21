namespace App.Application.Features.Auth.Dtos;

public sealed record RegisterInitiateRequest(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword);
