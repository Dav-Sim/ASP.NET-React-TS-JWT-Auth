using System.ComponentModel.DataAnnotations;

namespace Example.Auth.CustomValidationAttributes;

public class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string? password = value as string;

        if (string.IsNullOrWhiteSpace(password))
        {
            return new ValidationResult("Password is required");
        }

        if (password.Length < 6)
        {
            return new ValidationResult("Password must be at least 6 characters long");
        }

        if (!password.Any(char.IsUpper) || !password.Any(char.IsDigit))
        {
            return new ValidationResult("Password must contain at least one uppercase letter and one digit");
        }

        return ValidationResult.Success;
    }
}
