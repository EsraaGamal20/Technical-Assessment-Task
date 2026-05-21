namespace App.Infrastructure.Authentication;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly IDateTimeProvider _clock;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(IOptions<JwtSettings> settings, IDateTimeProvider clock)
    {
        _settings = settings.Value;
        _clock    = clock;

        if (string.IsNullOrWhiteSpace(_settings.SecretKey) || _settings.SecretKey.Length < 32)
            throw new InvalidOperationException(
                "JwtSettings:SecretKey must be at least 32 characters long.");

        var keyBytes = Encoding.UTF8.GetBytes(_settings.SecretKey);
        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);
    }

    public string GenerateAccessToken(ApplicationUser user)
    {
        var now     = _clock.UtcNow;
        var expires = now.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),

            new(AppClaimTypes.UserId,user.Id.ToString()),
            new(AppClaimTypes.Email,user.Email),
            new(AppClaimTypes.FullName,user.FullName),
            new(AppClaimTypes.PhoneNumber,user.PhoneNumber),
            new(AppClaimTypes.IsPhoneVerified, user.IsPhoneVerified.ToString().ToLowerInvariant())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims:claims,
            notBefore:now,
            expires:expires,
            signingCredentials: _signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        Span<byte> buffer = stackalloc byte[64];
        RandomNumberGenerator.Fill(buffer);
        return Base64UrlEncode(buffer);
    }

    public DateTime GetAccessTokenExpiration()
        => _clock.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

    private static string Base64UrlEncode(ReadOnlySpan<byte> bytes)
    {
        var s = Convert.ToBase64String(bytes);
        return s.TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
