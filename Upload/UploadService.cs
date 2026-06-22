using Common;
using Common.Models;
using Config;
using Microsoft.Extensions.Options;

namespace Upload;

public class UploadService
{
    private readonly UploadSettings _settings;

    private static readonly Dictionary<string, byte[]> MagicBytes = new()
    {
        [".jpg"] = [0xFF, 0xD8, 0xFF],
        [".jpeg"] = [0xFF, 0xD8, 0xFF],
        [".png"] = [0x89, 0x50, 0x4E, 0x47],
        [".gif"] = [0x47, 0x49, 0x46],
        [".webp"] = [0x52, 0x49, 0x46, 0x46], // RIFF header
        [".pdf"] = [0x25, 0x50, 0x44, 0x46],
    };

    private static readonly HashSet<string> AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private static readonly HashSet<string> AllowedFileExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif", ".pdf"];

    public UploadService(IOptions<UploadSettings> settings) { _settings = settings.Value; }

    public async Task<Result<string>> UploadImageAsync(IFormFile file, HttpRequest request)
    {
        if (file == null || file.Length == 0)
            return Result<string>.Fail("No file provided.");
        if (file.Length > _settings.ImageMaxSizeMb * 1024 * 1024)
            return Result<string>.Fail($"Image must be less than {_settings.ImageMaxSizeMb}MB.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext))
            return Result<string>.Fail("Only JPEG, PNG, WEBP, and GIF images are allowed.");

        // Validate magic bytes (file signature)
        if (!await ValidateMagicBytesAsync(file, ext))
            return Result<string>.Fail("File content does not match the expected image format.");

        var fileName = $"{Guid.NewGuid()}{ext}";
        var dir = Path.Combine(Directory.GetCurrentDirectory(), _settings.UploadsPath);
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, fileName);

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        var url = $"{request.Scheme}://{request.Host}/uploads/{fileName}";
        return Result<string>.Ok(url, "Image uploaded successfully.");
    }

    public async Task<Result<string>> UploadFileAsync(IFormFile file, HttpRequest request)
    {
        if (file == null || file.Length == 0)
            return Result<string>.Fail("No file provided.");
        if (file.Length > _settings.FileMaxSizeMb * 1024 * 1024)
            return Result<string>.Fail($"File must be less than {_settings.FileMaxSizeMb}MB.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedFileExtensions.Contains(ext))
            return Result<string>.Fail("Only JPEG, PNG, WEBP, GIF, and PDF files are allowed.");

        // Validate magic bytes
        if (!await ValidateMagicBytesAsync(file, ext))
            return Result<string>.Fail("File content does not match the expected format.");

        var fileName = $"{Guid.NewGuid()}{ext}";
        var dir = Path.Combine(Directory.GetCurrentDirectory(), _settings.UploadsPath);
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, fileName);

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        var url = $"{request.Scheme}://{request.Host}/uploads/{fileName}";
        return Result<string>.Ok(url, "File uploaded successfully.");
    }

    private static async Task<bool> ValidateMagicBytesAsync(IFormFile file, string extension)
    {
        if (!MagicBytes.TryGetValue(extension, out var expected))
            return false;

        var buffer = new byte[expected.Length];
        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, expected.Length));

        if (bytesRead < expected.Length)
            return false;

        return buffer.AsSpan(0, expected.Length).SequenceEqual(expected);
    }
}
