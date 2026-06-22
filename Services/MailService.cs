using Common.Models;
using Config;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Services;

public interface IMailService
{
    Task<Result<bool>> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
    Task<Result<bool>> SendVerificationEmailAsync(string to, string token, CancellationToken ct = default);
    Task<Result<bool>> SendPasswordResetEmailAsync(string to, string token, CancellationToken ct = default);
    Task<Result<bool>> SendWelcomeEmailAsync(string to, CancellationToken ct = default);
}

public class MailService : IMailService
{
    private readonly MailSettings _settings;
    private readonly ILogger<MailService> _logger;

    public MailService(IOptions<MailSettings> settings, ILogger<MailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<Result<bool>> SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            if (_settings.UseSsl)
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
            else
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);

            if (!string.IsNullOrEmpty(_settings.Username))
                await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            return Result<bool>.Ok(true, "Email sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} with subject {Subject}", to, subject);
            return Result<bool>.Fail($"Failed to send email: {ex.Message}", AppCodes.InternalError);
        }
    }

    public async Task<Result<bool>> SendVerificationEmailAsync(string to, string token, CancellationToken ct = default)
    {
        var subject = "Verify Your Email - Schools League";
        var body = $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="font-family: Arial, sans-serif; padding: 20px; background: #f5f5f5;">
                <div style="max-width: 600px; margin: 0 auto; background: white; border-radius: 8px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);">
                    <h1 style="color: #1a73e8; text-align: center;">Schools League</h1>
                    <h2 style="color: #333;">Welcome!</h2>
                    <p style="color: #555; font-size: 16px; line-height: 1.6;">
                        Thank you for registering. Please verify your email address by clicking the link below:
                    </p>
                    <div style="text-align: center; margin: 30px 0;">
                        <a href="{_settings.BaseUrl}/auth/verify-email?token={token}&email={to}"
                           style="background: #1a73e8; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold;">
                            Verify Email
                        </a>
                    </div>
                    <p style="color: #555; font-size: 14px;">
                        Or copy this link into your browser:<br>
                        <span style="color: #1a73e8;">{_settings.BaseUrl}/auth/verify-email?token={token}&email={to}</span>
                    </p>
                    <p style="color: #999; font-size: 12px; margin-top: 30px;">
                        This link will expire in 24 hours. If you didn't create an account, you can safely ignore this email.
                    </p>
                </div>
            </body>
            </html>
            """;
        return await SendEmailAsync(to, subject, body, ct);
    }

    public async Task<Result<bool>> SendPasswordResetEmailAsync(string to, string token, CancellationToken ct = default)
    {
        var subject = "Reset Your Password - Schools League";
        var body = $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="font-family: Arial, sans-serif; padding: 20px; background: #f5f5f5;">
                <div style="max-width: 600px; margin: 0 auto; background: white; border-radius: 8px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);">
                    <h1 style="color: #1a73e8; text-align: center;">Schools League</h1>
                    <h2 style="color: #333;">Password Reset Request</h2>
                    <p style="color: #555; font-size: 16px; line-height: 1.6;">
                        We received a request to reset your password. Click the link below to set a new password:
                    </p>
                    <div style="text-align: center; margin: 30px 0;">
                        <a href="{_settings.BaseUrl}/auth/reset-password?token={token}&email={to}"
                           style="background: #1a73e8; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold;">
                            Reset Password
                        </a>
                    </div>
                    <p style="color: #555; font-size: 14px;">
                        Or copy this link into your browser:<br>
                        <span style="color: #1a73e8;">{_settings.BaseUrl}/auth/reset-password?token={token}&email={to}</span>
                    </p>
                    <p style="color: #999; font-size: 12px; margin-top: 30px;">
                        This link will expire in 1 hour. If you didn't request a password reset, you can safely ignore this email.
                    </p>
                </div>
            </body>
            </html>
            """;
        return await SendEmailAsync(to, subject, body, ct);
    }

    public async Task<Result<bool>> SendWelcomeEmailAsync(string to, CancellationToken ct = default)
    {
        var subject = "Welcome to Schools League!";
        var body = $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="font-family: Arial, sans-serif; padding: 20px; background: #f5f5f5;">
                <div style="max-width: 600px; margin: 0 auto; background: white; border-radius: 8px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);">
                    <h1 style="color: #1a73e8; text-align: center;">Schools League</h1>
                    <h2 style="color: #333;">Welcome Aboard!</h2>
                    <p style="color: #555; font-size: 16px; line-height: 1.6;">
                        Your email has been verified successfully. You can now log in and start using Schools League.
                    </p>
                    <div style="text-align: center; margin: 30px 0;">
                        <a href="{_settings.BaseUrl}/auth/login"
                           style="background: #1a73e8; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-size: 16px; font-weight: bold;">
                            Log In
                        </a>
                    </div>
                </div>
            </body>
            </html>
            """;
        return await SendEmailAsync(to, subject, body, ct);
    }
}
