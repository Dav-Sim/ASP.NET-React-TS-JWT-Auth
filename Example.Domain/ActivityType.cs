namespace Example.Domain;

public class ActivityType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<Activity> Activities { get; set; } = new List<Activity>();

    public enum ActivityTypeEnum
    {
        Login = 1,
        Register = 2,
        RefreshToken = 3,
        Logout = 4,
        EmailVerification = 5,
        PasswordReset = 6,
        PasswordChange = 7,
        UserUpdate = 8
    }
}
