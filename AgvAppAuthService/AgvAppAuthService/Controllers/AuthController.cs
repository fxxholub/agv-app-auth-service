using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AgvAppAuthService.Models;
using AgvAppAuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgvAppAuthService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserDbContext.UserDbContext _dbContext;
    private readonly TokenService _tokenService;

    public AuthController(UserDbContext.UserDbContext dbContext, TokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Username == model.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = _tokenService.GenerateJwtToken(user);
        return Ok(new { Token = token, Role = user.Role });
    }

    [Authorize]
    [HttpGet("test")]
    public IActionResult Test()
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        return Ok(new { Message = $"Hello, {username}! You are an {role}." });
    }
}