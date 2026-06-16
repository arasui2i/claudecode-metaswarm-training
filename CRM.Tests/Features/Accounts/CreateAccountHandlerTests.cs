using CRM.Application.Features.Accounts.Create;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Accounts;

[TestFixture]
public class CreateAccountHandlerTests
{
    private Mock<IAccountRepository> _repo = null!;
    private CreateAccountHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IAccountRepository>();
        _handler = new CreateAccountHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_CreatesAccountAndReturnsId()
    {
        _repo.Setup(r => r.NameExistsAsync("Acme Corp", null, default)).ReturnsAsync(false);
        _repo.Setup(r => r.AddAsync(It.IsAny<Account>(), default)).Returns(Task.CompletedTask);

        var id = await _handler.Handle(new CreateAccountCommand("Acme Corp"), default);

        id.Should().NotBeEmpty();
        _repo.Verify(r => r.AddAsync(It.Is<Account>(a => a.Name == "Acme Corp"), default), Times.Once);
    }

    [Test]
    public async Task Handle_DuplicateName_ThrowsInvalidOperationException()
    {
        _repo.Setup(r => r.NameExistsAsync("Dup Corp", null, default)).ReturnsAsync(true);

        var act = () => _handler.Handle(new CreateAccountCommand("Dup Corp"), default);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*name*");
    }
}

[TestFixture]
public class CreateAccountValidatorTests
{
    private CreateAccountValidator _validator = null!;
    private Mock<IAccountRepository> _repo = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IAccountRepository>();
        _validator = new CreateAccountValidator(_repo.Object);
    }

    [Test]
    public async Task Validate_EmptyName_Fails()
    {
        _repo.Setup(r => r.NameExistsAsync(It.IsAny<string>(), null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateAccountCommand(""));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Test]
    public async Task Validate_ValidCommand_Passes()
    {
        _repo.Setup(r => r.NameExistsAsync("Good Corp", null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateAccountCommand("Good Corp"));
        result.IsValid.Should().BeTrue();
    }
}
