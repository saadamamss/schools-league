using Common;
using Common.Models;
using Config;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtSettings _jwt;
    private readonly IMailService _mail;

    private const string RefreshTokenProvider = "SchoolsLeague";
    private const string RefreshTokenName = "refresh_token";

    public AuthService(
        AppDbContext db,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IOptions<JwtSettings> jwt,
        IMailService mail)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt.Value;
        _mail = mail;
    }

    // ── Register ────────────────────────────────────────────────���──
    public async Task<Result<object>> RegisterAsync(RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) != null)
            return Result<object>.Conflict("Email already in use.");

        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            PhoneNumber = dto.Phone,
            FullName = $"{dto.FirstName} {dto.MiddleName} {dto.LastName}".Replace("  ", " "),
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            SaId = dto.SaId,
            Gender = dto.Gender,
            CityId = dto.CityId,
            UserTypeId = dto.UserTypeId,
        };

        // Wrap critical steps in a transaction for atomicity
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<object>.ValidationError(string.Join(", ", errors));
            }

            await _userManager.AddToRoleAsync(user, "User");

            // Generate email verification token
            var verifyToken = GenerateSecureToken();
            user.VerifyToken = verifyToken;
            user.VerifyTokenExpiry = DateTime.UtcNow.AddDays(1);
            await _userManager.UpdateAsync(user);

            await transaction.CommitAsync();

            // Send verification email (fire-and-forget — don't block registration on email)
            _ = _mail.SendVerificationEmailAsync(dto.Email, verifyToken);

            return Result<object>.Ok(
                new { message = "Registration successful. Please check your email to verify your account." },
                "Registration successful.",
                AppCodes.Created);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ── Verify Email ────────────────────────────────────────────────
    public async Task<Result<AuthResponseDto>> VerifyEmailAsync(VerifyEmailQuery query)
    {
        var user = await _userManager.FindByEmailAsync(query.Email);
        if (user == null)
            return Result<AuthResponseDto>.NotFound("User not found.");

        if (user.EmailConfirmed)
            return Result<AuthResponseDto>.Fail("Email already verified.", AppCodes.ValidationError);

        if (user.VerifyToken != query.Token || user.VerifyTokenExpiry < DateTime.UtcNow)
            return Result<AuthResponseDto>.Fail("Invalid or expired verification token.", AppCodes.InvalidToken);

        user.EmailConfirmed = true;
        user.VerifyToken = null; // Consume the token
        user.VerifyTokenExpiry = null;
        await _userManager.UpdateAsync(user);

        // Send welcome email
        _ = _mail.SendWelcomeEmailAsync(query.Email);

        var token = await GenerateTokenAsync(user);
        return Result<AuthResponseDto>.Ok(new AuthResponseDto { Token = token, User = MapUserDto(user) }, "Email verified successfully.");
    }

    // ── Legacy verify-registration endpoint (frontend still uses this path) ──
    public async Task<Result<AuthResponseDto>> VerifyRegistrationAsync(VerifyRegistrationDto dto)
    {
        // Forward to the token-based verification
        return await VerifyEmailAsync(new VerifyEmailQuery { Email = dto.Email, Token = dto.Token });
    }

    // ── Login ───────────────────────────────────────────────────────
    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Result<AuthResponseDto>.Fail("Invalid credentials.", AppCodes.InvalidCredentials);
        if (user.IsDeleted)
            return Result<AuthResponseDto>.Fail("Invalid credentials.", AppCodes.InvalidCredentials);
        if (!user.EmailConfirmed)
            return Result<AuthResponseDto>.Fail("Please verify your email first.", AppCodes.EmailNotVerified);

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
        if (result.IsLockedOut)
            return Result<AuthResponseDto>.Fail("Account locked. Try again later.", AppCodes.AccountLocked);
        if (!result.Succeeded)
            return Result<AuthResponseDto>.Fail("Invalid credentials.", AppCodes.InvalidCredentials);

        var token = await GenerateTokenAsync(user);

        // Generate and store refresh token
        var refreshToken = GenerateSecureToken();
        await _userManager.SetAuthenticationTokenAsync(user, RefreshTokenProvider, RefreshTokenName, refreshToken);

        return Result<AuthResponseDto>.Ok(
            new AuthResponseDto
            {
                Token = token,
                User = MapUserDto(user),
                RefreshToken = refreshToken,
            },
            "Login successful.");
    }

    // ── Refresh Token (cookie-based) ────────────────────────────────
    public async Task<Result<AuthResponseDto>> RefreshAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return Result<AuthResponseDto>.Unauthorized("Refresh token is required.");

        // Find a user who has this refresh token
        var userTokens = await _db.UserTokens
            .Where(t => t.LoginProvider == RefreshTokenProvider
                     && t.Name == RefreshTokenName
                     && t.Value == refreshToken)
            .ToListAsync();

        if (userTokens.Count == 0)
            return Result<AuthResponseDto>.Unauthorized("Invalid refresh token.");

        var user = await _userManager.FindByIdAsync(userTokens[0].UserId.ToString());
        if (user == null || !user.IsActive || user.IsDeleted)
            return Result<AuthResponseDto>.Unauthorized("User not found or inactive.");

        // Rotate refresh token: remove old, create new
        await _userManager.RemoveAuthenticationTokenAsync(user, RefreshTokenProvider, RefreshTokenName);
        var newRefreshToken = GenerateSecureToken();
        await _userManager.SetAuthenticationTokenAsync(user, RefreshTokenProvider, RefreshTokenName, newRefreshToken);

        var token = await GenerateTokenAsync(user);
        return Result<AuthResponseDto>.Ok(
            new AuthResponseDto
            {
                Token = token,
                User = MapUserDto(user),
                RefreshToken = newRefreshToken,
            },
            "Token refreshed.");
    }

    // ── Logout ──────────────────────────────────────────────────────
    public async Task<Result<object>> LogoutAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result<object>.NotFound("User not found.");

        // Remove refresh token
        await _userManager.RemoveAuthenticationTokenAsync(user, RefreshTokenProvider, RefreshTokenName);

        // Increment TokenVersion to invalidate all existing JWTs
        user.TokenVersion++;
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result<object>.Ok(new { message = "Logged out successfully." }, "Logged out successfully.");
    }

    // ── Forgot Password ─────────────────────────────────────────────
    public async Task<Result<object>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        // Always return success (don't reveal if email exists)
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || user.IsDeleted)
            return Result<object>.Ok(new { message = "If the email exists, a reset link has been sent." });

        var resetToken = GenerateSecureToken();
        user.ResetToken = resetToken;
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _userManager.UpdateAsync(user);

        _ = _mail.SendPasswordResetEmailAsync(dto.Email, resetToken);

        return Result<object>.Ok(new { message = "If the email exists, a reset link has been sent." });
    }

    // ── Verify Password Reset OTP ────────────────────────────────────
    public async Task<Result<object>> VerifyOtpAsync(VerifyOtpDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || user.IsDeleted)
            return Result<object>.Fail("Invalid or expired OTP.", AppCodes.InvalidToken);

        if (user.ResetToken != dto.Otp || user.ResetTokenExpiry < DateTime.UtcNow)
            return Result<object>.Fail("Invalid or expired OTP.", AppCodes.InvalidToken);

        // Consume the token — prevents replay attack
        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        await _userManager.UpdateAsync(user);

        return Result<object>.Ok(new { message = "OTP verified successfully." });
    }

    // ── Reset Password ──────────────────────────────────────────────
    public async Task<Result<object>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || user.IsDeleted)
            return Result<object>.Fail("Invalid or expired reset token.", AppCodes.InvalidToken);

        if (user.ResetToken != dto.Token || user.ResetTokenExpiry < DateTime.UtcNow)
            return Result<object>.Fail("Invalid or expired reset token.", AppCodes.InvalidToken);

        // Reset password
        var resetResult = await _userManager.RemovePasswordAsync(user);
        if (!resetResult.Succeeded)
            return Result<object>.Fail("Failed to reset password.", AppCodes.InternalError);

        var addResult = await _userManager.AddPasswordAsync(user, dto.Password);
        if (!addResult.Succeeded)
        {
            var errors = addResult.Errors.Select(e => e.Description).ToList();
            return Result<object>.ValidationError(string.Join(", ", errors));
        }

        // Consume the reset token
        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        user.TokenVersion++; // Invalidate all existing sessions
        user.UpdatedAt = DateTime.UtcNow;

        // Remove all refresh tokens for this user
        var userTokens = await _db.UserTokens
            .Where(t => t.UserId == user.Id && t.LoginProvider == RefreshTokenProvider)
            .ToListAsync();
        _db.UserTokens.RemoveRange(userTokens);

        await _userManager.UpdateAsync(user);
        await _db.SaveChangesAsync();

        return Result<object>.Ok(new { message = "Password reset successfully. Please log in with your new password." });
    }

    // ── Resend Verification ─────────────────────────────────────────
    public async Task<Result<object>> ResendVerificationAsync(ResendVerificationDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || user.IsDeleted)
            return Result<object>.Ok(new { message = "If the email exists, a verification link has been sent." });

        if (user.EmailConfirmed)
            return Result<object>.Fail("Email is already verified.", AppCodes.ValidationError);

        var verifyToken = GenerateSecureToken();
        user.VerifyToken = verifyToken;
        user.VerifyTokenExpiry = DateTime.UtcNow.AddDays(1);
        await _userManager.UpdateAsync(user);

        _ = _mail.SendVerificationEmailAsync(dto.Email, verifyToken);

        return Result<object>.Ok(new { message = "Verification email sent. Please check your inbox." });
    }

    // ── Change Password ─────────────────────────────────────────────
    public async Task<Result<object>> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result<object>.NotFound("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result<object>.ValidationError(string.Join(", ", errors));
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result<object>.Ok(new { message = "Password changed successfully." }, "Password changed successfully.");
    }

    // ── Get Profile ─────────────────────────────────────────────────
    public async Task<Result<UserDto>> GetProfileAsync(string userId)
    {
        var user = await _db.Users
            .Include(u => u.City)
            .Include(u => u.UserType)
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);
        if (user == null)
            return Result<UserDto>.NotFound("User not found.");

        var dto = MapUserDto(user);

        var today = TimeHelper.Today;
        var todayAttendance = await _db.Attendances
            .FirstOrDefaultAsync(a => a.UserId == user.Id && a.Date == today);
        if (todayAttendance != null)
        {
            dto.TodayCheckIn = todayAttendance.CheckIn?.ToString("HH:mm");
            dto.TodayCheckOut = todayAttendance.CheckOut?.ToString("HH:mm");
        }

        return Result<UserDto>.Ok(dto);
    }

    // ── Token Generation ────────────────────────────────────────────
    public async Task<string> GenerateTokenAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.FullName),
            new("token_version", user.TokenVersion.ToString()),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _jwt.Issuer,
            _jwt.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(_jwt.ExpireDays),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ── Helpers ─────────────────────────────────────────────────────
    private static string GenerateSecureToken()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("/", "_")
            .Replace("+", "-")
            .Replace("=", "");
    }

    private static UserDto MapUserDto(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        FirstName = user.FirstName,
        MiddleName = user.MiddleName,
        LastName = user.LastName,
        Email = user.Email!,
        Phone = user.PhoneNumber!,
        SaId = user.SaId,
        Gender = user.Gender,
        ProfileImage = user.ProfileImage,
        IsActive = user.IsActive,
        BirthDate = user.BirthDate,
        CreatedAt = user.CreatedAt,
        City = user.City != null
            ? new CityDto { Id = user.City.Id, Name = new() { ["ar"] = user.City.NameAr, ["en"] = user.City.NameEn } }
            : null,
        UserType = user.UserType != null
            ? new UserTypeDto
            {
                Id = user.UserType.Id,
                Code = user.UserType.Code,
                Name = new() { ["ar"] = user.UserType.NameAr, ["en"] = user.UserType.NameEn },
                Key = user.UserType.Key,
                IsActive = user.UserType.IsActive,
            }
            : null,
        BankInfo = user.BankName != null
            ? new BankInfoDto
            {
                BankName = user.BankName,
                AccountNumber = user.AccountNumber,
                Iban = user.Iban,
                SwiftCode = user.SwiftCode,
            }
            : null,
    };
}
