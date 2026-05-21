namespace App.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var raw = Principal?.FindFirst(AppClaimTypes.UserId)?.Value
                   ?? Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public string? Email => Principal?.FindFirst(AppClaimTypes.Email)?.Value;

    public Guid GetUserIdOrThrow()
        => UserId ?? throw new UnauthorizedException("User is not authenticated.");
}
