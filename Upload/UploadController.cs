using Upload;
using Common;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace UploadNamespace;

[ApiController]
[Route("api/dashboard/[controller]")]
[SwaggerTag("Upload")]
[Authorize]
[EnableRateLimiting("upload")]
public class UploadController : BaseController
{
    private readonly UploadService _upload;

    public UploadController(UploadService upload) { _upload = upload; }

    [HttpPost("image")]
    [SwaggerOperation("Upload an image")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> UploadImage(IFormFile file) =>
        (await _upload.UploadImageAsync(file, Request)).ToActionResult();

    [HttpPost("file")]
    [SwaggerOperation("Upload a file")]
    public async Task<IActionResult> UploadFile(IFormFile file) =>
        (await _upload.UploadFileAsync(file, Request)).ToActionResult();
}
