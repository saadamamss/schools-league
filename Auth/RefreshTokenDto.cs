using System.ComponentModel.DataAnnotations;

namespace Auth;

public class RefreshTokenDto
{
    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; set; } = string.Empty;
}
