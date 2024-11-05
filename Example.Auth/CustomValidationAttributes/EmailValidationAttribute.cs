using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Example.Auth.CustomValidationAttributes;

public class EmailValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string? email = value as string;

        if (string.IsNullOrWhiteSpace(email))
        {
            return new ValidationResult("Email is required");
        }

        if (!Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
        {
            return new ValidationResult("Email is not valid");
        }

        return ValidationResult.Success;
    }
}
