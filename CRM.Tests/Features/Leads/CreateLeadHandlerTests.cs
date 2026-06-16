using CRM.Application.DTOs;
using CRM.Application.Features.Leads.Create;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Leads;

[TestFixture]
public class CreateLeadHandlerTests
{
    private Mock<ILeadRepository> _repo = null!;
    private CreateLeadHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ILeadRepository>();
        _handler = new CreateLeadHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_CreatesLeadAndReturnsId()
    {
        _repo.Setup(r => r.EmailExistsAsync("lead@acme.com", null, default)).ReturnsAsync(false);
        _repo.Setup(r => r.AddAsync(It.IsAny<Lead>(), default)).Returns(Task.CompletedTask);

        var cmd = new CreateLeadCommand("Jane", "Doe", "lead@acme.com");
        var id = await _handler.Handle(cmd, default);

        id.Should().NotBeEmpty();
        _repo.Verify(r => r.AddAsync(It.Is<Lead>(l => l.Email == "lead@acme.com"), default), Times.Once);
    }

    [Test]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _repo.Setup(r => r.EmailExistsAsync("dup@acme.com", null, default)).ReturnsAsync(true);

        var cmd = new CreateLeadCommand("Jane", "Doe", "dup@acme.com");
        var act = () => _handler.Handle(cmd, default);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*email*");
    }
}

[TestFixture]
public class CreateLeadValidatorTests
{
    private CreateLeadValidator _validator = null!;
    private Mock<ILeadRepository> _repo = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ILeadRepository>();
        _validator = new CreateLeadValidator(_repo.Object);
    }

    [Test]
    public async Task Validate_EmptyFirstName_Fails()
    {
        _repo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateLeadCommand("", "Doe", "a@b.com"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Test]
    public async Task Validate_InvalidEmail_Fails()
    {
        var result = await _validator.ValidateAsync(new CreateLeadCommand("Jane", "Doe", "not-an-email"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Test]
    public async Task Validate_ValidCommand_Passes()
    {
        _repo.Setup(r => r.EmailExistsAsync("good@crm.com", null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateLeadCommand("Jane", "Doe", "good@crm.com"));
        result.IsValid.Should().BeTrue();
    }
}
