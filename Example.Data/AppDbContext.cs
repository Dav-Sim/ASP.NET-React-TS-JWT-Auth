using Microsoft.EntityFrameworkCore;
using Example.Domain;

namespace Example.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Activity> Activities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("user", "example");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id)
                .UseIdentityColumn(1, 1);

            e.HasIndex(e => e.Email)
                .IsUnique();

            e.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(250);

            e.Property(x => x.EmailVerificationToken)
                .HasMaxLength(250);

            e.Property(x => x.FirstName)
                .HasMaxLength(250);

            e.Property(x => x.LastName)
                .HasMaxLength(250);

            e.Property(x => x.RegistrationDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasConversion(
                    v => v,
                    v => new DateTime(v.Ticks, DateTimeKind.Utc));

            e.Property(x => x.EmailVerificationDate)
                .HasConversion(
                    v => v,
                    v => v.HasValue ? new DateTime(v.Value.Ticks, DateTimeKind.Utc) : null);

            e.Property(x => x.EmailVerificationTokenValidFrom)
                .HasConversion(
                    v => v,
                    v => v.HasValue ? new DateTime(v.Value.Ticks, DateTimeKind.Utc) : null);

            e.Property(x => x.PasswordHash)
                .HasMaxLength(1000);

            e.Property(x => x.PasswordSalt)
                .HasMaxLength(1000);

            e.HasMany(x => x.RefreshTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.ToTable("refresh_token", "example");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id)
                .UseIdentityColumn(1, 1);

            e.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(250);

            e.Property(x => x.Expires)
                .IsRequired()
                .HasConversion(
                    v => v,
                    v => new DateTime(v.Ticks, DateTimeKind.Utc));

            e.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Activity>(e =>
        {
            e.ToTable("activity", "example");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id)
                .UseIdentityColumn(1, 1);

            e.Property(x => x.UserId)
                .IsRequired();

            e.Property(x => x.Date)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasConversion(
                    v => v,
                    v => new DateTime(v.Ticks, DateTimeKind.Utc));

            e.Property(x => x.ActivityTypeId)
                .IsRequired();

            e.HasOne(x => x.User)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.ActivityType)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.ActivityTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActivityType>(e =>
        {
            e.ToTable("activity_type", "example");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id)
                .UseIdentityColumn(1, 1);

            e.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            e.HasMany(x => x.Activities)
                .WithOne(x => x.ActivityType)
                .HasForeignKey(x => x.ActivityTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActivityType>().HasData(new ActivityType[] {
                new ActivityType { Id = 1, Name = "Login", Description = "Login using name and password" },
                new ActivityType { Id = 2, Name = "Register", Description = "Register a new user" },
                new ActivityType { Id = 3, Name = "RefreshToken", Description = "Refresh token" },
                new ActivityType { Id = 4, Name = "Logout", Description = "Logout" },
                new ActivityType { Id = 5, Name = "EmailVerification", Description = "Email verification" },
                new ActivityType { Id = 6, Name = "PasswordReset", Description = "Password reset" },
                new ActivityType { Id = 7, Name = "PasswordChange", Description = "Password change" },
                new ActivityType { Id = 8, Name = "UserUpdate", Description = "User update" },
            });

        base.OnModelCreating(modelBuilder);
    }
}
