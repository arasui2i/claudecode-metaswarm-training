using CRM.Application.Features.Auth.Login;
using FluentAssertions;
using NUnit.Framework;

namespace CRM.Tests.Features.Auth;

[TestFixture]
public class LoginCommandValidatorTests
{
    private LoginCommandValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new LoginCommandValidator();

    [Test]
    public void Validate_EmptyEmailOrUsername_Fails()
    {
        var result = _validator.Validate(new LoginCommand("", "password123", false));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EmailOrUsername");
    }

    [Test]
    public void Validate_EmptyPassword_Fails()
    {
        var result = _validator.Validate(new LoginCommand("user@crm.com", "", false));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Test]
    public void Validate_PasswordTooShort_Fails()
    {
        var result = _validator.Validate(new LoginCommand("user@crm.com", "abc", false));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Test]
    public void Validate_ValidCommand_Passes()
    {
        var result = _validator.Validate(new LoginCommand("user@crm.com", "password123", false));
        result.IsValid.Should().BeTrue();
    }
}
