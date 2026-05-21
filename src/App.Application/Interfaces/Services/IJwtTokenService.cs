using App.Domain.Entities;

namespace App.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user);
    string GenerateRefreshToken();
    DateTime GetAccessTokenExpiration();
}
