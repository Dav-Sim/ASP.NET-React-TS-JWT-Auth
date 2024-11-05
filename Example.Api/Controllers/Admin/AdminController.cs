using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Example.Api.Controllers.Admin.Dtos;
using Example.Auth;
using Example.Auth.Services;
using Example.Data;

namespace Example.Api.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserService _userService;

    public AdminController(AppDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet("user")]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _context
            .Users
            .Select(a => new UserDto
            {
                Id = a.Id,
                Email = a.Email,
                EmailVerified = a.EmailVerified,
                EmailVerificationDate = a.EmailVerificationDate,
                FirstName = a.FirstName,
                LastName = a.LastName,
                RegistrationDate = a.RegistrationDate,
                IsAdmin = a.IsAdmin
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("user/{id}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] int id)
    {
        var user = await _context
            .Users
            .Where(a => a.Id == id)
            .Select(a => new UserDto
            {
                Id = a.Id,
                Email = a.Email,
                EmailVerified = a.EmailVerified,
                EmailVerificationDate = a.EmailVerificationDate,
                FirstName = a.FirstName,
                LastName = a.LastName,
                RegistrationDate = a.RegistrationDate,
                IsAdmin = a.IsAdmin
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpDelete("user/{id}")]
    public async Task<ActionResult> DeleteUser([FromRoute] int id)
    {
        var user = await _userService.GetAuthenticatedUserAsync();

        if (user == null)
        {
            return NotFound();
        }

        if (user.Id == id)
        {
            return BadRequest("You cannot delete yourself");
        }

        var userToDelete = await _context.Users
            .Include(a => a.RefreshTokens)
            .Include(a => a.Activities)
            .ThenInclude(a => a.ActivityType)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (userToDelete == null)
        {
            return NotFound();
        }

        _context.Users.Remove(userToDelete);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("user/{id}")]
    public async Task<ActionResult> UpdateUser([FromRoute] int id, [FromBody] UserDto userToUpdate)
    {
        var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        user.Email = userToUpdate.Email;
        if (user.EmailVerified == false && userToUpdate.EmailVerified == true && user.EmailVerificationDate == null)
        {
            user.EmailVerificationDate = DateTime.UtcNow;
        }
        user.EmailVerified = userToUpdate.EmailVerified;
        user.FirstName = userToUpdate.FirstName;
        user.LastName = userToUpdate.LastName;
        user.IsAdmin = userToUpdate.IsAdmin;
        await _context.SaveChangesAsync();

        return Ok();
    }
}
