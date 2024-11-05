using Example.Auth.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Example.Auth.Dtos;

public class VerifyEmailRequestDto
{
    [EmailValidation]
    public string Email { get; set; } = null!;
    [Required]
    public string Token { get; set; } = null!;
}
