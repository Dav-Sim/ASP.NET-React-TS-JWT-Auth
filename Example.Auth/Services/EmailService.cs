
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Example.Auth.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly AuthSettings _settings;

    public EmailService(ILogger<EmailService> logger, IWebHostEnvironment env, IOptions<AuthSettings> options)
    {
        _settings = options.Value;
        _logger = logger;
        _env = env;
    }

    public async Task SendEmailVerificationAsync(string email, string emailVerificationToken)
    {
        var url = $"{_settings.ApplicationUrl}/verifyemail?email={email}&token={emailVerificationToken}";

        var subject = "Email Verification - example.davisimanek.cz";

        var body = $@"<p>Please click the following link to verify your email address:</p>
                     <p><a href=""{url}"">{url}</a></p>
                     <p>If you did not request this email, please report abuse to <a href=""mailto:X@Y.Z"">X@Y.Z</a></p>";

        if (!_env.IsProduction())
        {
            _logger.LogInformation(message: $"Sending email verification to {email} with body '{body}'");
        }
        else
        {
            await SendEmailAsync(email, subject, body);
        }
    }

    public async Task SendPasswordResetAsync(string email, string passwordResetToken)
    {
        var url = $"{_settings.ApplicationUrl}/resetpassword?email={email}&token={passwordResetToken}";

        var subject = "Password Reset - example.davisimanek.cz";

        var body = $@"<p>Please click the following link to reset your password:</p>
                     <p><a href=""{url}"">{url}</a></p>
                     <p>If you did not request this email, please report abuse to <a href=""mailto:X@Y.Z"">X@Y.Z</a></p>";

        if (!_env.IsProduction())
        {
            _logger.LogInformation(message: $"Sending email verification to {email} with body '{body}'");
        }
        else
        {
            await SendEmailAsync(email, subject, body);
        }
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Email.SmtpServer, _settings.Email.Port, true);

        await client.AuthenticateAsync(_settings.Email.From, _settings.Email.Password);

        //MailboxAddress sender = client.Verify(_settings.Email.From);
        var sender = new MailboxAddress(_settings.Email.From, _settings.Email.From);

        var message = new MimeMessage();
        message.From.Add(sender);
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;

        message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = htmlBody
        };

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}
