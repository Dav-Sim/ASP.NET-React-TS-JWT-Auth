namespace Example.Domain;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime? EmailVerificationDate { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenValidFrom { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public virtual List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual List<Activity> Activities { get; set; } = new List<Activity>();

    public TimeSpan GetEmailVerificationTokenAge()
    {
        if (EmailVerificationTokenValidFrom.HasValue)
        {
            return DateTime.UtcNow - EmailVerificationTokenValidFrom.Value;
        }
        else
        {
            return TimeSpan.MaxValue;
        }
    }
}
