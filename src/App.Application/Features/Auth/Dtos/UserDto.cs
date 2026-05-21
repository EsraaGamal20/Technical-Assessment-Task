namespace App.Application.Features.Auth.Dtos;

public sealed record UserDto(
    Guid Id,
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsPhoneVerified,
    bool IsEmailVerified,
    DateTime CreatedAt);
