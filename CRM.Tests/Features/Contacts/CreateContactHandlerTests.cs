using CRM.Application.Features.Contacts.Create;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Contacts;

[TestFixture]
public class CreateContactHandlerTests
{
    private Mock<IContactRepository> _repo = null!;
    private CreateContactHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IContactRepository>();
        _handler = new CreateContactHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_CreatesContactAndReturnsId()
    {
        _repo.Setup(r => r.EmailExistsAsync("jane@acme.com", null, default)).ReturnsAsync(false);
        _repo.Setup(r => r.AddAsync(It.IsAny<Contact>(), default)).Returns(Task.CompletedTask);

        var cmd = new CreateContactCommand("Jane", "Doe", "jane@acme.com");
        var id = await _handler.Handle(cmd, default);

        id.Should().NotBeEmpty();
        _repo.Verify(r => r.AddAsync(It.Is<Contact>(c => c.Email == "jane@acme.com"), default), Times.Once);
    }

    [Test]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _repo.Setup(r => r.EmailExistsAsync("dup@acme.com", null, default)).ReturnsAsync(true);

        var act = () => _handler.Handle(new CreateContactCommand("Jane", "Doe", "dup@acme.com"), default);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*email*");
    }
}

[TestFixture]
public class CreateContactValidatorTests
{
    private CreateContactValidator _validator = null!;
    private Mock<IContactRepository> _repo = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IContactRepository>();
        _validator = new CreateContactValidator(_repo.Object);
    }

    [Test]
    public async Task Validate_EmptyFirstName_Fails()
    {
        _repo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateContactCommand("", "Doe", "a@b.com"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Test]
    public async Task Validate_InvalidEmail_Fails()
    {
        var result = await _validator.ValidateAsync(new CreateContactCommand("Jane", "Doe", "not-an-email"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Test]
    public async Task Validate_ValidCommand_Passes()
    {
        _repo.Setup(r => r.EmailExistsAsync("good@crm.com", null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateContactCommand("Jane", "Doe", "good@crm.com"));
        result.IsValid.Should().BeTrue();
    }
}
