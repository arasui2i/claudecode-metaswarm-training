using CRM.Application.DTOs;
using CRM.Application.Features.Customers.Create;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Customers;

[TestFixture]
public class CreateCustomerHandlerTests
{
    private Mock<ICustomerRepository> _repo = null!;
    private CreateCustomerHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ICustomerRepository>();
        _handler = new CreateCustomerHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_CreatesCustomerAndReturnsId()
    {
        _repo.Setup(r => r.EmailExistsAsync("john@acme.com", null, default)).ReturnsAsync(false);
        _repo.Setup(r => r.AddAsync(It.IsAny<Customer>(), default)).Returns(Task.CompletedTask);

        var cmd = new CreateCustomerCommand("John", "Doe", "john@acme.com") { Company = "Acme" };
        var id = await _handler.Handle(cmd, default);

        id.Should().NotBeEmpty();
        _repo.Verify(r => r.AddAsync(It.Is<Customer>(c => c.Email == "john@acme.com"), default), Times.Once);
    }

    [Test]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _repo.Setup(r => r.EmailExistsAsync("dup@acme.com", null, default)).ReturnsAsync(true);

        var cmd = new CreateCustomerCommand("Jane", "Doe", "dup@acme.com");
        var act = () => _handler.Handle(cmd, default);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*email*");
    }
}

[TestFixture]
public class CreateCustomerValidatorTests
{
    private CreateCustomerValidator _validator = null!;
    private Mock<ICustomerRepository> _repo = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ICustomerRepository>();
        _validator = new CreateCustomerValidator(_repo.Object);
    }

    [Test]
    public async Task Validate_EmptyFirstName_Fails()
    {
        _repo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateCustomerCommand("", "Doe", "a@b.com"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Test]
    public async Task Validate_InvalidEmailFormat_Fails()
    {
        var result = await _validator.ValidateAsync(new CreateCustomerCommand("John", "Doe", "not-an-email"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Test]
    public async Task Validate_ValidCommand_Passes()
    {
        _repo.Setup(r => r.EmailExistsAsync("good@crm.com", null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateCustomerCommand("John", "Doe", "good@crm.com"));
        result.IsValid.Should().BeTrue();
    }
}
