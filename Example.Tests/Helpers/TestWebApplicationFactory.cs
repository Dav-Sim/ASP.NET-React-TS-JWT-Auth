using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Example.Api;
using Example.Auth.Services;
using Example.Data;

namespace Example.Tests.Helpers;

public class TestWebApplicationFactory<T> : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            TestWebApplicationFactory<T>.SetupDatabase(services);
            TestWebApplicationFactory<T>.SetupEmailService(services);
        });
    }

    private static void SetupDatabase(IServiceCollection services)
    {
        // Unregister existing database service (SQL Server).
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

        if (descriptor != null) services.Remove(descriptor);

        // Register new in-memory database
        services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("memory"));
    }

    private static void SetupEmailService(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(IEmailService));

        if (descriptor != null) services.Remove(descriptor);

        Constants.EmailService = new Mock<IEmailService>(MockBehavior.Loose);

        services.AddTransient((s) => Constants.EmailService.Object);
    }
}
