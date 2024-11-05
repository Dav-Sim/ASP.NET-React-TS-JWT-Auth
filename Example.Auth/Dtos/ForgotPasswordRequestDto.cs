using Example.Auth.CustomValidationAttributes;

namespace Example.Auth.Dtos;
public class ForgotPasswordRequestDto
{
    [EmailValidation]
    public string Email { get; set; } = null!;
}
