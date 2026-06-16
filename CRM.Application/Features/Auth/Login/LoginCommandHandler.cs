using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Auth.Login;

public class LoginCommandHandler(IUserRepository userRepository, IJwtService jwtService)
    : IRequestHandler<LoginCommand, LoginCommandResult>
{
    public async Task<LoginCommandResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetWithRolesAsync(request.EmailOrUsername, cancellationToken);

        // Always call BCrypt.Verify to prevent timing-based user enumeration
        var hash = user?.PasswordHash ?? BCrypt.Net.BCrypt.HashPassword("dummy");
        var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, hash);

        if (user is null || !passwordValid || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var jwt = jwtService.GenerateToken(user, roles, request.RememberMe);

        return new LoginCommandResult(
            jwt.AccessToken,
            jwt.ExpiresAt,
            new LoginUserDto(user.Id, user.Email, user.Username, roles));
    }
}
