using CRM.Application.Features.Auth.Login;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Auth;

[TestFixture]
public class LoginCommandHandlerTests
{
    private Mock<IUserRepository> _userRepo = null!;
    private Mock<IJwtService> _jwtService = null!;
    private LoginCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepo = new Mock<IUserRepository>();
        _jwtService = new Mock<IJwtService>();
        _handler = new LoginCommandHandler(_userRepo.Object, _jwtService.Object);
    }

    [Test]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@crm.com",
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            IsActive = true,
            UserRoles = [new UserRole { Role = new Role { Name = "Admin" } }]
        };
        _userRepo.Setup(r => r.GetWithRolesAsync("admin@crm.com", default))
                 .ReturnsAsync(user);
        _jwtService.Setup(j => j.GenerateToken(user, It.IsAny<IEnumerable<string>>(), false))
                   .Returns(new JwtResult("token-xyz", DateTime.UtcNow.AddDays(1)));

        var command = new LoginCommand("admin@crm.com", "password123", false);
        var result = await _handler.Handle(command, default);

        result.AccessToken.Should().Be("token-xyz");
        result.User.Email.Should().Be("admin@crm.com");
    }

    [Test]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        var user = new User
        {
            Email = "admin@crm.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password"),
            IsActive = true
        };
        _userRepo.Setup(r => r.GetWithRolesAsync("admin@crm.com", default))
                 .ReturnsAsync(user);

        var command = new LoginCommand("admin@crm.com", "wrong-password", false);
        var act = () => _handler.Handle(command, default);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task Handle_UnknownUser_ThrowsUnauthorized()
    {
        _userRepo.Setup(r => r.GetWithRolesAsync(It.IsAny<string>(), default))
                 .ReturnsAsync((User?)null);

        var command = new LoginCommand("nobody@crm.com", "password123", false);
        var act = () => _handler.Handle(command, default);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task Handle_RememberMe_PassesRememberMeToJwtService()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@crm.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            IsActive = true,
            UserRoles = []
        };
        _userRepo.Setup(r => r.GetWithRolesAsync("admin@crm.com", default))
                 .ReturnsAsync(user);
        _jwtService.Setup(j => j.GenerateToken(user, It.IsAny<IEnumerable<string>>(), true))
                   .Returns(new JwtResult("long-token", DateTime.UtcNow.AddDays(7)));

        var command = new LoginCommand("admin@crm.com", "password123", true);
        var result = await _handler.Handle(command, default);

        _jwtService.Verify(j => j.GenerateToken(user, It.IsAny<IEnumerable<string>>(), true), Times.Once);
        result.AccessToken.Should().Be("long-token");
    }
}
