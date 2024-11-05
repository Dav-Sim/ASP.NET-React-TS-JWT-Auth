using Example.Auth.CustomValidationAttributes;

namespace Example.Auth.Dtos;
public class ChangePasswordRequestDto
{
    public string Password { get; set; } = null!;
    [PasswordValidation]
    public string NewPassword { get; set; } = null!;
}
