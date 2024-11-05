namespace Example.Auth;

public static class AppRoles
{
    public const string Admin = "admin";
    public const string User = "user";
    public const string NotVerified = "notverified";

    public const string AnyValidRole = $"{Admin},{User}";
}
