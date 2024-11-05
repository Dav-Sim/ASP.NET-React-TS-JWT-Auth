using Example.Api.Middleware;
using Example.Auth;
using Example.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Example.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        if (builder.Environment.EnvironmentName != "Testing")
        {
            builder.Configuration
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Secrets.json", optional: false, reloadOnChange: true);
        }

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        if (builder.Environment.EnvironmentName != "Testing")
        {
            builder.Host.UseSerilog((context, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.Debug(Serilog.Events.LogEventLevel.Information));
        }

        builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));

        builder.Services.AddUserService();

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), opts =>
            {
                opts.MigrationsHistoryTable("__EFMigrationsHistory", "example");
            });
        });

        //authentication
        builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

        // Configure the HTTP request pipeline.
        var app = builder.Build();

        app.UseErrorHandler();
        app.UseDefaultFiles();
        app.UseStaticFiles();

        if (!app.Environment.IsEnvironment("Testing"))
        {
            app.UseSerilogRequestLogging();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        if (app.Environment.IsDevelopment()) app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        if (app.Environment.IsDevelopment()) app.Use(async (context, next) =>
        {
            await Task.Delay(200);
            await next();
        });

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
