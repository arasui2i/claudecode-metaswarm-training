using CRM.Application.Features.Auth.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(
                new LoginCommand(request.EmailOrUsername, request.Password, request.RememberMe),
                cancellationToken);

            // Set JWT as HttpOnly cookie to prevent XSS token theft
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresAt
            };
            Response.Cookies.Append("auth_token", result.AccessToken, cookieOptions);

            return Ok(new
            {
                result.ExpiresAt,
                result.User
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid email/username or password." });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token");
        return NoContent();
    }
}

public record LoginRequest(string EmailOrUsername, string Password, bool RememberMe);
