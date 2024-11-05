using Example.Auth.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Example.Auth.Dtos;
public class AuthenticateRequestDto
{
    [EmailValidation]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}
