using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Example.Auth.Dtos;
using Example.Data;
using Example.Domain;
using System.Security.Claims;

namespace Example.Auth.Services;

public class UserService
{
    private readonly IHttpContextAccessor _http;
    private readonly AppDbContext _context;
    private readonly PasswordService _passwordVerificationService;
    private readonly TokenProvider _tokenProvider;
    private readonly IEmailService _emailService;

    public UserService(IHttpContextAccessor http, AppDbContext context, PasswordService passwordVerificationService, TokenProvider tokenProvider, IEmailService emailService)
    {
        _http = http;
        _context = context;
        _passwordVerificationService = passwordVerificationService;
        _tokenProvider = tokenProvider;
        _emailService = emailService;
    }

    public async Task<User?> GetAuthenticatedUserAsync()
    {
        var userEmail = _http.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        var user = await _context.Users
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Email == userEmail);

        await RemoveExpiredRefreshTokensAsync(user);

        return user;
    }

    public async Task<IEnumerable<Activity>> GetActivitiesAsync(int userId)
    {
        var user = await _context.Users
            .Include(x => x.Activities)
            .ThenInclude(x => x.ActivityType)
            .SingleAsync(a => a.Id == userId);

        return user.Activities;
    }

    public async Task ChangePasswordAsync(int userId, string password, bool isReset)
    {
        var user = await _context.Users
            .Include(a => a.RefreshTokens)
            .SingleAsync(a => a.Id == userId);

        user.EmailVerificationToken = null;

        string salt = _passwordVerificationService.GenerateSalt();
        string hash = _passwordVerificationService.Hash(password, salt);

        user.PasswordSalt = salt;
        user.PasswordHash = hash;

        _context.RefreshTokens.RemoveRange(user.RefreshTokens);

        if (isReset)
        {
            _context.Activities.Add(new Activity(user, ActivityType.ActivityTypeEnum.PasswordReset));
        }
        else
        {
            _context.Activities.Add(new Activity(user, ActivityType.ActivityTypeEnum.PasswordChange));
        }

        await _context.SaveChangesAsync();
    }

    public async Task<User> CreateUserAsync(string email, string password, string? firstName, string? lastName)
    {
        var user = new User
        {
            Email = email,
            EmailVerified = false,
            FirstName = firstName,
            LastName = lastName,
        };

        string salt = _passwordVerificationService.GenerateSalt();
        string hash = _passwordVerificationService.Hash(password, salt);

        user.PasswordSalt = salt;
        user.PasswordHash = hash;

        _context.Users.Add(user);

        _context.Activities.Add(new Activity(user, ActivityType.ActivityTypeEnum.Register));

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<(string accessToken, RefreshToken refreshToken)> GenerateUserTokensAsync(int userId)
    {
        var user = await _context.Users.SingleAsync(a => a.Id == userId);

        var accessToken = _tokenProvider.GenerateToken(user);
        var refreshToken = _tokenProvider.GenerateRefreshToken(user);

        user.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetUserByExpiredAccessTokenAsync(string accessToken)
    {
        var principal = _tokenProvider.GetPrincipalFromPossiblyExpiredToken(accessToken);

        if (principal == null)
        {
            return null;
        }

        var userEmail = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        return await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
    }

    public async Task SendEmailVerificationAsync(int id)
    {
        var user = await _context.Users.SingleAsync(a => a.Id == id);

        user.EmailVerificationToken = Guid.NewGuid().ToString();
        user.EmailVerificationTokenValidFrom = DateTime.UtcNow;

        await _emailService.SendEmailVerificationAsync(user.Email, user.EmailVerificationToken);

        await _context.SaveChangesAsync();
    }

    public async Task SendForgotPasswordEmailVerificationAsync(int userId)
    {
        var user = await _context.Users.SingleAsync(a => a.Id == userId);

        user.EmailVerificationToken = Guid.NewGuid().ToString();
        user.EmailVerificationTokenValidFrom = DateTime.UtcNow;

        await _emailService.SendPasswordResetAsync(user.Email, user.EmailVerificationToken);

        await _context.SaveChangesAsync();
    }

    public async Task RevokeRefreshTokenAsync(int userId, string refreshToken)
    {
        var user = await _context.Users
            .Include(x => x.RefreshTokens)
            .SingleAsync(a => a.Id == userId);

        var token = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);

        if (token != null)
        {
            user.RefreshTokens.Remove(token);

            _context.Activities.Add(new Activity(user, ActivityType.ActivityTypeEnum.Logout));

            await _context.SaveChangesAsync();
        }
    }

    public async Task RevokeAllRefreshTokensAsync(int userId)
    {
        var user = await _context.Users
            .Include(x => x.RefreshTokens)
            .SingleAsync(a => a.Id == userId);

        _context.RefreshTokens.RemoveRange(user.RefreshTokens);

        _context.Activities.Add(new Activity(user, ActivityType.ActivityTypeEnum.Logout));

        await _context.SaveChangesAsync();
    }

    public async Task<bool> ValidateRefreshTokenAndDeleteItAsync(int userId, string refreshToken)
    {
        var user = await _context.Users
            .Include(x => x.RefreshTokens)
            .SingleAsync(a => a.Id == userId);

        var token = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);

        if (token == null)
        {
            return false;
        }

        if (token.Expires < DateTime.UtcNow)
        {
            user.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
            return false;
        }

        user.RefreshTokens.Remove(token);

        _context.Activities.Add(new Activity(user, ActivityType.ActivityTypeEnum.RefreshToken));

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task LogLoginActivityAsync(int id)
    {
        var user = await _context.Users.SingleAsync(a => a.Id == id);

        _context.Activities.Add(new Activity(user, ActivityType.ActivityTypeEnum.Login));

        await _context.SaveChangesAsync();
    }

    public async Task MarkEmailAsVerifiedAsync(int userId)
    {
        var user = await _context.Users.SingleAsync(a => a.Id == userId);
        user.EmailVerified = true;
        user.EmailVerificationDate = DateTime.UtcNow;
        user.EmailVerificationToken = null;

        _context.Activities.Add(new Activity(user, ActivityType.ActivityTypeEnum.EmailVerification));

        await _context.SaveChangesAsync();
    }

    public bool VerifyPassword(User user, string password)
    {
        return _passwordVerificationService.Verify(password, user.PasswordSalt, user.PasswordHash);
    }

    public async Task UpdateUserProfileAsync(int id, UserProfileToUpdateDto profile)
    {
        var user = await _context.Users
            .SingleAsync(a => a.Id == id);

        user.FirstName = profile.FirstName;
        user.LastName = profile.LastName;

        _context.Activities.Add(new Activity(user, ActivityType.ActivityTypeEnum.UserUpdate));

        await _context.SaveChangesAsync();
    }

    private async Task RemoveExpiredRefreshTokensAsync(User? user)
    {
        if (user == null)
        {
            return;
        }

        var expiredTokens = user.RefreshTokens
            .Where(x => x.Expires < DateTime.UtcNow)
            .ToList();

        if (expiredTokens.Count > 0)
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
