namespace App.Application.Features.Auth.Dtos;

public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAt,
    int ExpiresInSeconds,
    UserDto User);
