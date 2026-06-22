using System.Text.Json;
using Data;

namespace SchoolsLeague.Tests;

public class DtoSerializationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Fact]
    public void VerifyRegistrationDto_AcceptsOtpField()
    {
        var json = """{"email":"test@test.com","otp":"abc123"}""";
        var dto = JsonSerializer.Deserialize<VerifyRegistrationDto>(json, JsonOptions);

        Assert.NotNull(dto);
        Assert.Equal("test@test.com", dto.Email);
        Assert.Equal("abc123", dto.Token);
    }

    [Fact]
    public void ResetPasswordDto_AcceptsOtpAndPasswordConfirmation()
    {
        var json = """{"email":"test@test.com","otp":"abc123","password":"NewPass1","password_confirmation":"NewPass1"}""";
        var dto = JsonSerializer.Deserialize<ResetPasswordDto>(json, JsonOptions);

        Assert.NotNull(dto);
        Assert.Equal("abc123", dto.Token);
        Assert.Equal("NewPass1", dto.Password);
        Assert.Equal("NewPass1", dto.PasswordConfirmation);
    }

    [Fact]
    public void VerifyOtpDto_SerializesCorrectly()
    {
        var json = """{"email":"user@test.com","otp":"123456"}""";
        var dto = JsonSerializer.Deserialize<VerifyOtpDto>(json, JsonOptions);

        Assert.NotNull(dto);
        Assert.Equal("user@test.com", dto.Email);
        Assert.Equal("123456", dto.Otp);
    }

    [Fact]
    public void RefreshRequestDto_AcceptsToken()
    {
        var json = """{"token":"my-refresh-token"}""";
        var dto = JsonSerializer.Deserialize<RefreshRequestDto>(json, JsonOptions);

        Assert.NotNull(dto);
        Assert.Equal("my-refresh-token", dto.Token);
    }
}
