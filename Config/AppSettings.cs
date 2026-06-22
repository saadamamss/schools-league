namespace Config;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpireDays { get; set; } = 7;
}

public class SeedSettings
{
    public string AdminPassword { get; set; } = "Admin@123";
    public string UserPassword { get; set; } = "User@123";
}

public class OtpSettings
{
    public int ExpireMinutes { get; set; } = 5;
    public int MaxAttempts { get; set; } = 5;
    public int LockoutMinutes { get; set; } = 15;
}

public class CorsSettings
{
    public string[] Origins { get; set; } = [];
}

public class UploadSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string UploadsPath { get; set; } = "wwwroot/uploads";
    public int ImageMaxSizeMb { get; set; } = 5;
    public int FileMaxSizeMb { get; set; } = 10;
}

public class MailSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string DisplayName { get; set; } = "Schools League";
    public bool UseSsl { get; set; } = false;
    public string BaseUrl { get; set; } = string.Empty;
}

public class TimeZoneSettings
{
    public string TimeZone { get; set; } = "Asia/Riyadh";
}
