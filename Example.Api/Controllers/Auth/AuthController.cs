using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Example.Auth;
using Example.Auth.Dtos;
using Example.Auth.Services;
using Example.Domain;

namespace Example.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthSettings _authSettings;
    private readonly UserService _userService;
    private readonly IHttpContextAccessor _http;

    public AuthController(IOptions<AuthSettings> options, UserService userService, IHttpContextAccessor http)
    {
        _authSettings = options.Value;
        _userService = userService;
        _http = http;
    }

    /// <summary>
    /// Get user profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> Profile()
    {
        var user = await _userService.GetAuthenticatedUserAsync();

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new UserProfileDto
        (
            email: user.Email,
            emailVerified: user.EmailVerified,
            firstName: user.FirstName,
            lastName: user.LastName,
            registrationDate: user.RegistrationDate,
            isAdmin: user.IsAdmin,
            emailVerificationDate: user.EmailVerificationDate
        ));
    }

    [HttpGet("my-activities")]
    [Authorize]
    public async Task<ActionResult<List<ActivityDto>>> GetMyActivities()
    {
        var user = await _userService.GetAuthenticatedUserAsync();

        if (user == null)
        {
            return Unauthorized();
        }

        var activities = await _userService.GetActivitiesAsync(user.Id);

        return Ok(activities.Select(a => new ActivityDto(a.ActivityType.Name, a.ActivityType.Description, a.Date)).ToList());
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize(Roles = AppRoles.AnyValidRole)]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UserProfileToUpdateDto profile)
    {
        var user = await _userService.GetAuthenticatedUserAsync();

        if (user == null)
        {
            return Unauthorized();
        }

        await _userService.UpdateUserProfileAsync(user.Id, profile);

        return Ok();
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<UserProfileDto>> Login([FromBody] AuthenticateRequestDto request)
    {
        var user = await _userService.GetUserByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized();
        }

        bool passwordMatch = _userService.VerifyPassword(user, request.Password);

        if (!passwordMatch)
        {
            return Unauthorized();
        }

        var (accessToken, refreshToken) = await _userService.GenerateUserTokensAsync(user.Id);

        await _userService.LogLoginActivityAsync(user.Id);

        AppendAccessTokenCookie(accessToken);

        AppendRefreshTokenCookie(refreshToken);

        return Ok(new UserProfileDto
        (
            email: user.Email,
            emailVerified: user.EmailVerified,
            firstName: user.FirstName,
            lastName: user.LastName,
            registrationDate: user.RegistrationDate,
            isAdmin: user.IsAdmin,
            emailVerificationDate: user.EmailVerificationDate
        ));
    }

    /// <summary>
    /// Register new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<UserProfileDto>> Register([FromBody] RegisterRequestDto request)
    {
        var existingUser = await _userService.GetUserByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return BadRequest("Email already registered");
        }

        var user = await _userService.CreateUserAsync(request.Email, request.Password, request.FirstName, request.LastName);

        await _userService.SendEmailVerificationAsync(user.Id);

        return Ok(new UserProfileDto
        (
            email: user.Email,
            emailVerified: user.EmailVerified,
            firstName: user.FirstName,
            lastName: user.LastName,
            registrationDate: user.RegistrationDate,
            isAdmin: user.IsAdmin,
            emailVerificationDate: user.EmailVerificationDate
        ));
    }

    /// <summary>
    /// Verify email address using email verification token
    /// </summary>
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request)
    {
        var user = await _userService.GetUserByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized();
        }

        if (user.EmailVerificationToken == null || user.EmailVerificationToken != request.Token)
        {
            return Unauthorized();
        }

        if (user.EmailVerificationTokenValidFrom.HasValue && user.EmailVerificationTokenValidFrom.Value.AddMinutes(_authSettings.EmailVerificationTokenExpirationMinutes) < DateTime.UtcNow)
        {
            return BadRequest("Token expired");
        }

        await _userService.MarkEmailAsVerifiedAsync(user.Id);

        ClearAuthCookies();

        return Ok();
    }

    /// <summary>
    /// Resend email verification token
    /// </summary>
    [HttpPost("resend-email-verification")]
    public async Task<IActionResult> ResendEmailVerification([FromBody] ResendEmailVerificationRequestDto request)
    {
        var user = await _userService.GetUserByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized();
        }

        // Prevent spamming, only allow to resend email verification once per...
        if (user.GetEmailVerificationTokenAge() < TimeSpan.FromSeconds(_authSettings.EmailVerificationResendSeconds))
        {
            return StatusCode(StatusCodes.Status429TooManyRequests);
        }

        await _userService.SendEmailVerificationAsync(user.Id);

        return Ok();
    }

    /// <summary>
    /// Forgot password, send email with verification token
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var user = await _userService.GetUserByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized();
        }

        // Prevent spamming, only allow to resend email verification once per...
        if (user.GetEmailVerificationTokenAge() < TimeSpan.FromSeconds(_authSettings.EmailVerificationResendSeconds))
        {
            return StatusCode(StatusCodes.Status429TooManyRequests);
        }

        await _userService.SendForgotPasswordEmailVerificationAsync(user.Id);

        return Ok();
    }

    /// <summary>
    /// Reset forgotten password using email verification token generated by forgot password
    /// </summary>
    [HttpPost("reset-forgotten-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var user = await _userService.GetUserByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized();
        }

        if (user.EmailVerificationToken == null || user.EmailVerificationToken != request.Token)
        {
            return Unauthorized();
        }

        if (user.GetEmailVerificationTokenAge().TotalMinutes > _authSettings.EmailVerificationTokenExpirationMinutes)
        {
            return BadRequest("Token expired");
        }

        await _userService.ChangePasswordAsync(user.Id, request.Password, true);

        var (accessToken, refreshToken) = await _userService.GenerateUserTokensAsync(user.Id);

        AppendAccessTokenCookie(accessToken);

        AppendRefreshTokenCookie(refreshToken);

        return Ok();
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize(Roles = AppRoles.AnyValidRole)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var user = await _userService.GetAuthenticatedUserAsync();

        if (user == null)
        {
            return Unauthorized();
        }

        bool passwordMatch = _userService.VerifyPassword(user, request.Password);
        if (!passwordMatch)
        {
            return BadRequest("Invalid old password");
        }

        await _userService.ChangePasswordAsync(user.Id, request.NewPassword, false);

        var (accessToken, refreshToken) = await _userService.GenerateUserTokensAsync(user.Id);

        AppendAccessTokenCookie(accessToken);

        AppendRefreshTokenCookie(refreshToken);

        return Ok();
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var (accessToken, refreshToken) = GetTokensFromCookies();

        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized();
        }

        var user = await _userService.GetUserByExpiredAccessTokenAsync(accessToken);

        if (user == null)
        {
            return Unauthorized();
        }

        if (!await _userService.ValidateRefreshTokenAndDeleteItAsync(user.Id, refreshToken))
        {
            return Unauthorized();
        }

        var tokens = await _userService.GenerateUserTokensAsync(user.Id);

        AppendAccessTokenCookie(tokens.accessToken);

        AppendRefreshTokenCookie(tokens.refreshToken);

        return Ok();

    }

    /// <summary>
    /// Revoke token
    /// </summary>
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken()
    {
        var (accessToken, refreshToken) = GetTokensFromCookies();

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized();
        }

        var user = await _userService.GetUserByExpiredAccessTokenAsync(accessToken);

        if (user == null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(refreshToken))
        {
            await _userService.RevokeAllRefreshTokensAsync(user.Id);
        }
        else
        {
            await _userService.RevokeRefreshTokenAsync(user.Id, refreshToken);
        }

        ClearAuthCookies();

        return Ok();
    }

    private void ClearAuthCookies()
    {
        _http.HttpContext?.Response.Cookies.Delete("refreshToken", new()
        {
            Path = "/api/auth"
        });
        _http.HttpContext?.Response.Cookies.Delete("accessToken");
    }

    private void AppendAccessTokenCookie(string accessToken)
    {
        _http.HttpContext?.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(31),
        });
    }

    private void AppendRefreshTokenCookie(RefreshToken refreshToken)
    {
        _http.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth",
            Expires = DateTime.UtcNow.AddDays(31),
        });
    }

    private (string? accessToken, string? refreshToken) GetTokensFromCookies()
    {
        var accessToken = _http.HttpContext?.Request.Cookies["accessToken"];
        var refreshToken = _http.HttpContext?.Request.Cookies["refreshToken"];

        return (accessToken, refreshToken);
    }
}
