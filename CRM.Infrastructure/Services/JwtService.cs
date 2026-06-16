using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Infrastructure.Services;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public JwtResult GenerateToken(User user, IEnumerable<string> roles, bool rememberMe)
    {
        // JWT__KEY must come from environment variables — never hardcoded in appsettings.json
        var key = configuration["JWT__KEY"]
            ?? throw new InvalidOperationException("JWT__KEY environment variable is not set.");

        if (key.Length < 32)
            throw new InvalidOperationException("JWT__KEY must be at least 32 characters.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("username", user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var expiryDays = rememberMe ? 7 : 1;
        var expiresAt = DateTime.UtcNow.AddDays(expiryDays);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["JWT__ISSUER"],
            audience: configuration["JWT__AUDIENCE"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
