using Auth;
using Common;
using Common.Models;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthNamespace;

[ApiController]
[Route("api/dashboard/[controller]")]
[SwaggerTag("Authentication")]
public class AuthController : BaseController
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth) { _auth = auth; }

    private const string RefreshCookieName = "refresh_token";

    /// <summary>
    /// Register a new user. A verification email will be sent.
    /// </summary>
    [HttpPost("register")]
    [EnableRateLimiting("register")]
    [SwaggerOperation("Register a new user")]
    [ProducesResponseType(typeof(ApiResponse<object>), 201)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto) =>
        (await _auth.RegisterAsync(dto)).ToActionResult();

    /// <summary>
    /// Verify email with a token (sent via email link).
    /// </summary>
    [HttpPost("verify-email")]
    [EnableRateLimiting("otp_verify")]
    [SwaggerOperation("Verify email with token")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailQuery query)
    {
        var result = await _auth.VerifyEmailAsync(query);
        return result.ToActionResult();
    }

    /// <summary>
    /// Legacy verify-registration endpoint (accepts token instead of OTP now).
    /// </summary>
    [HttpPost("verify-registration")]
    [EnableRateLimiting("otp_verify")]
    [SwaggerOperation("Verify registration with token")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> VerifyRegistration([FromBody] VerifyRegistrationDto dto)
    {
        var result = await _auth.VerifyRegistrationAsync(dto);
        return result.ToActionResult();
    }

    /// <summary>
    /// Login with email and password. Sets refresh token as HttpOnly cookie.
    /// </summary>
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [SwaggerOperation("Login with email and password")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _auth.LoginAsync(dto);
        if (!result.IsSuccess) return result.ToActionResult();

        // Set refresh token as HttpOnly cookie
        if (result.Data?.RefreshToken != null)
            SetRefreshCookie(RefreshCookieName, result.Data.RefreshToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Refresh JWT token using HttpOnly refresh cookie (or request body fallback). Returns new JWT and rotates the refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [SwaggerOperation("Refresh JWT token")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto? body = null)
    {
        var refreshToken = Request.Cookies[RefreshCookieName] ?? body?.Token;
        var result = await _auth.RefreshAsync(refreshToken);
        if (!result.IsSuccess) return result.ToActionResult();

        if (result.Data?.RefreshToken != null)
            SetRefreshCookie(RefreshCookieName, result.Data.RefreshToken);

        return result.ToActionResult();
    }

    /// <summary>
    /// Logout the current user. Clears refresh token and invalidates all JWTs.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [SwaggerOperation("Logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var result = await _auth.LogoutAsync(CurrentUserId);
        ClearRefreshCookie(RefreshCookieName);
        return result.ToActionResult();
    }

    /// <summary>
    /// Request a password reset email. Sends an OTP to the user's email.
    /// </summary>
    [HttpPost("password/forgot")]
    [EnableRateLimiting("password_reset")]
    [SwaggerOperation("Request password reset")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto) =>
        (await _auth.ForgotPasswordAsync(dto)).ToActionResult();

    /// <summary>
    /// Verify the password reset OTP.
    /// </summary>
    [HttpPost("password/verify-otp")]
    [EnableRateLimiting("otp_verify")]
    [SwaggerOperation("Verify password reset OTP")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto) =>
        (await _auth.VerifyOtpAsync(dto)).ToActionResult();

    /// <summary>
    /// Reset password using a verified OTP.
    /// </summary>
    [HttpPost("password/reset")]
    [EnableRateLimiting("password_reset")]
    [SwaggerOperation("Reset password")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto) =>
        (await _auth.ResetPasswordAsync(dto)).ToActionResult();

    /// <summary>
    /// Resend verification email.
    /// </summary>
    [HttpPost("resend-registration-otp")]
    [SwaggerOperation("Resend verification email")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto) =>
        (await _auth.ResendVerificationAsync(dto)).ToActionResult();

    /// <summary>
    /// Change password for the current user.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [SwaggerOperation("Change password")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto) =>
        (await _auth.ChangePasswordAsync(CurrentUserId, dto)).ToActionResult();

    /// <summary>
    /// Get current user profile.
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    [SwaggerOperation("Get current user profile")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Profile() =>
        (await _auth.GetProfileAsync(CurrentUserId.ToString())).ToActionResult();

    // ── Cookie helpers ──────────────────────────────────────────────
    private bool UseSecureCookie =>
        HttpContext.Request.IsHttps;

    private void SetRefreshCookie(string name, string value)
    {
        Response.Cookies.Append(name, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = UseSecureCookie,
            SameSite = SameSiteMode.Strict,
            Path = "/api/dashboard/auth",
            MaxAge = TimeSpan.FromDays(7),
        });
    }

    private void ClearRefreshCookie(string name)
    {
        Response.Cookies.Append(name, string.Empty, new CookieOptions
        {
            HttpOnly = true,
            Secure = UseSecureCookie,
            SameSite = SameSiteMode.Strict,
            Path = "/api/dashboard/auth",
            Expires = DateTimeOffset.UnixEpoch,
        });
    }
}
