using Example.Auth.Services;
using Example.Data;
using Example.Domain;

namespace Example.Tests.Helpers;

public class TestDataSeeder
{
    public static async Task SeedTestDataAsync(AppDbContext dbContext)
    {
        var pwdService = new PasswordService();
        var salt = pwdService.GenerateSalt();
        var users = new List<User>
        {
            //verified valid user
            new User
            {
                Email = Constants.ExistingVerifiedUser.email,
                EmailVerified = true,
                EmailVerificationDate = DateTime.UtcNow,
                PasswordHash = pwdService.Hash(Constants.ExistingVerifiedUser.password, salt),
                PasswordSalt = salt
            },
            //unverified user
            new User
            {
                Email = Constants.ExistingNotVerifiedUser.email,
                EmailVerified = false,
                EmailVerificationDate = null,
                PasswordHash = pwdService.Hash(Constants.ExistingNotVerifiedUser.password, salt),
                PasswordSalt = salt
            }
         };

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.SaveChangesAsync();
    }
}
