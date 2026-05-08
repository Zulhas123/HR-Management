using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IConfiguration configuration) : ControllerBase
{
    public sealed record TokenRequest(string Username, string Password);

    [HttpPost("token")]
    public IActionResult Token([FromBody] TokenRequest request)
    {
        // Minimal MVP auth: single admin user configured in appsettings or env vars.
        var adminUser = configuration["Admin:Username"] ?? "admin";
        var adminPass = configuration["Admin:Password"] ?? "admin123";

        if (!string.Equals(request.Username, adminUser, StringComparison.Ordinal) ||
            !string.Equals(request.Password, adminPass, StringComparison.Ordinal))
        {
            return Unauthorized();
        }

        var jwt = configuration.GetSection("Jwt");
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;
        var key = jwt["Key"]!;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, request.Username),
            new(ClaimTypes.Role, "Admin"),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return Ok(new { access_token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}

