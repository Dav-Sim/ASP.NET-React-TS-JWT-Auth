using Example.Auth.CustomValidationAttributes;

namespace Example.Auth.Dtos;
public class RegisterRequestDto
{
    [EmailValidation]
    public string Email { get; set; } = null!;
    [PasswordValidation]
    public string Password { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
