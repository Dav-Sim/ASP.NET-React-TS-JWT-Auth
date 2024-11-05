using Moq;
using Example.Auth.Services;

namespace Example.Tests.Helpers
{
    internal class Constants
    {
        public static (string email, string password) ExistingVerifiedUser = ("test@example.com", "Password123!");
        public static (string email, string password) ExistingNotVerifiedUser = ("notverifiedtest@example.com", "Password456!");
        public static Mock<IEmailService> EmailService = null!;
    }
}
