using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register/initiate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RegisterInitiate(
        [FromBody] RegisterInitiateRequest request,
        CancellationToken ct)
    {
        var result = await _auth.RegisterInitiateAsync(request, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result)
            : ToActionResult(result);
    }

    [HttpPost("register/verify-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpRequest request,
        CancellationToken ct)
    {
        var result = await _auth.VerifyOtpAsync(request, ct);
        return ToActionResult(result);
    }


    [HttpPost("register/resend-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ResendOtp(
        [FromBody] ResendOtpRequest request,
        CancellationToken ct)
    {
        var result = await _auth.ResendOtpAsync(request, ct);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken ct)
    {
        var result = await _auth.LoginAsync(request, ct);
        return ToActionResult(result);
    }
}
