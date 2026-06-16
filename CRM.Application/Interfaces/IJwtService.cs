using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public record JwtResult(string AccessToken, DateTime ExpiresAt);

public interface IJwtService
{
    JwtResult GenerateToken(User user, IEnumerable<string> roles, bool rememberMe);
}
