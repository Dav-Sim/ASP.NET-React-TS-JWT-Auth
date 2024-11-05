namespace Example.Auth.Dtos;

public class UserProfileDto
{
    private UserProfileDto() { }
    public UserProfileDto(string email, bool emailVerified, string? firstName, string? lastName, DateTime registrationDate, bool isAdmin, DateTime? emailVerificationDate)
    {
        Email = email;
        EmailVerified = emailVerified;
        FirstName = firstName;
        LastName = lastName;
        RegistrationDate = registrationDate;
        IsAdmin = isAdmin;
        EmailVerificationDate = emailVerificationDate;
    }

    public string Email { get; set; } = null!;
    public bool EmailVerified { get; set; }
    public DateTime? EmailVerificationDate { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool IsAdmin { get; set; }
}
