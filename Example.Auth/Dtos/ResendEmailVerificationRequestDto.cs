using Example.Auth.CustomValidationAttributes;

namespace Example.Auth.Dtos;
public class ResendEmailVerificationRequestDto
{
    [EmailValidation]
    public string Email { get; set; } = null!;
}
