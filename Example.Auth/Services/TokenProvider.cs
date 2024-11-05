using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Example.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Example.Auth.Services;

public class TokenProvider
{
    private readonly AuthSettings _authSettings;

    public TokenProvider(IOptions<AuthSettings> options)
    {
        _authSettings = options.Value;
    }

    internal string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authSettings.Jwt.Secret);

        var claims = new ClaimsIdentity(new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("email_verified", user.EmailVerified.ToString()),
        });

        if (user.EmailVerified != true)
            claims.AddClaim(new Claim(ClaimTypes.Role, AppRoles.NotVerified));
        else
            claims.AddClaim(new Claim(ClaimTypes.Role, AppRoles.User));

        if (user.IsAdmin)
            claims.AddClaim(new Claim(ClaimTypes.Role, AppRoles.Admin));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(_authSettings.Jwt.AccessExpirationMinutes),
            Audience = _authSettings.Jwt.Audience,
            Issuer = _authSettings.Jwt.Issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    internal RefreshToken GenerateRefreshToken(User user)
    {
        using var generator = RandomNumberGenerator.Create();
        var randomBytes = new byte[64];
        generator.GetBytes(randomBytes);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(_authSettings.RefreshTokenExpirationDays),
            User = user,
        };
    }

    internal ClaimsPrincipal? GetPrincipalFromPossiblyExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authSettings.Jwt.Secret);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _authSettings.Jwt.Issuer,
                ValidAudience = _authSettings.Jwt.Audience,
                ValidateLifetime = false //DONT validate lifetime here
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
