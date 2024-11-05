namespace Example.Auth;

public class AuthSettings
{
    public JwtSettings Jwt { get; set; } = null!;
    public EmailSettings Email { get; set; } = null!;
    /// <summary>
    /// Email token exipres in minutes
    /// </summary>
    public int EmailVerificationTokenExpirationMinutes { get; set; }
    /// <summary>
    /// Seconds to wait before resending email verification
    /// </summary>
    public int EmailVerificationResendSeconds { get; set; }
    /// <summary>
    /// Refresh token expiration in days
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; }
    /// <summary>
    /// Web app base url
    /// </summary>
    public string ApplicationUrl { get; set; } = null!;

    public class JwtSettings
    {
        public string Secret { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int AccessExpirationMinutes { get; set; }
    }

    public class EmailSettings
    {
        public string From { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string SmtpServer { get; set; } = null!;
        public int Port { get; set; }
    }
}
