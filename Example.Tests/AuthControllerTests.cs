using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Example.Api;
using Example.Auth.Dtos;
using Example.Data;
using Example.Tests.Helpers;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Example.Tests;

public class AuthControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly AppDbContext _context;
    private readonly IServiceScope _scope;

    public AuthControllerTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;

        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        TestDataSeeder.SeedTestDataAsync(_context).Wait();
    }

    public void Dispose()
    {
        _context.Dispose();
        _scope.Dispose();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new AuthenticateRequestDto
        {
            Email = Constants.ExistingVerifiedUser.email,
            Password = Constants.ExistingVerifiedUser.password
        };

        // Act
        var response = await client.PostAsync("/api/auth/login", GetStringContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(response.Headers.GetValues("Set-Cookie"), c => c.Contains("accessToken"));
        Assert.Contains(response.Headers.GetValues("Set-Cookie"), c => c.Contains("refreshToken"));
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new AuthenticateRequestDto
        {
            Email = Constants.ExistingVerifiedUser.email,
            Password = "wrongpassword"
        };

        // Act
        var response = await client.PostAsync("/api/auth/login", GetStringContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_UnverifiedEmail_ReturnsProfileWithUnverifiedFlag()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new AuthenticateRequestDto
        {
            Email = Constants.ExistingNotVerifiedUser.email,
            Password = Constants.ExistingNotVerifiedUser.password
        };

        // Act
        var response = await client.PostAsync("/api/auth/login", GetStringContent(request));
        var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(profile!.EmailVerified == false);
    }

    [Fact]
    public async Task Register_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new RegisterRequestDto
        {
            Email = "newemail@example.com",
            Password = "ydDBVFBpa126ssword"
        };

        // Act
        var response = await client.PostAsync("/api/auth/register", GetStringContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_ValidCredentials_VerificationEmailWasSent()
    {
        // Arrange
        Constants.EmailService.Invocations.Clear();
        var client = _factory.CreateClient();
        var request = new RegisterRequestDto
        {
            Email = "newemail@example.com",
            Password = "ydDBVFBpa126ssword"
        };

        // Act
        var response = await client.PostAsync("/api/auth/register", GetStringContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Constants.EmailService.Verify(s => s.SendEmailVerificationAsync(It.Is<string>(a => a == request.Email), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Register_ExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new RegisterRequestDto
        {
            Email = Constants.ExistingVerifiedUser.email.ToUpper(),
            Password = "password"
        };

        // Act
        var response = await client.PostAsync("/api/auth/register", GetStringContent(request));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new RegisterRequestDto
        {
            Email = "",
            Password = "password"
        };

        // Act
        var response = await client.PostAsync("/api/auth/register", GetStringContent(request));
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problemDetails?.Errors["Email"]);
    }

    private StringContent GetStringContent(object obj)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(obj);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
