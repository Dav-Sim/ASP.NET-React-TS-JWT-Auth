
namespace Example.Auth.Services
{
    public interface IEmailService
    {
        Task SendEmailVerificationAsync(string email, string emailVerificationToken);
        Task SendPasswordResetAsync(string email, string passwordResetToken);
    }
}