using Example.Auth.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Example.Auth.Dtos;
public class ResetPasswordRequestDto
{
    [EmailValidation]
    public string Email { get; set; } = null!;
    [Required]
    public string Token { get; set; } = null!;
    [PasswordValidation]
    public string Password { get; set; } = null!;
}
