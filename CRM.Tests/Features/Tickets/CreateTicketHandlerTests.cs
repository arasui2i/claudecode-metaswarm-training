using CRM.Application.Features.Tickets.CreateTicket;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Tickets;

[TestFixture]
public class CreateTicketHandlerTests
{
    private Mock<ITicketRepository> _repo = null!;
    private CreateTicketHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ITicketRepository>();
        _handler = new CreateTicketHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_GeneratesTicketNumberAndReturnsId()
    {
        _repo.Setup(r => r.GetNextTicketNumberAsync(default)).ReturnsAsync("TKT-00001");
        _repo.Setup(r => r.AddAsync(It.IsAny<Ticket>(), default)).Returns(Task.CompletedTask);

        var id = await _handler.Handle(new CreateTicketCommand("Server down"), default);

        id.Should().NotBeEmpty();
        _repo.Verify(r => r.AddAsync(
            It.Is<Ticket>(t => t.TicketNumber == "TKT-00001" && t.Subject == "Server down"),
            default), Times.Once);
    }

    [Test]
    public async Task Handle_WithAccountAndContact_SetsRelationships()
    {
        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        _repo.Setup(r => r.GetNextTicketNumberAsync(default)).ReturnsAsync("TKT-00002");
        _repo.Setup(r => r.AddAsync(It.IsAny<Ticket>(), default)).Returns(Task.CompletedTask);

        var cmd = new CreateTicketCommand("Login issue")
        {
            AccountId = accountId,
            ContactId = contactId,
            Priority = TicketPriority.High,
        };
        await _handler.Handle(cmd, default);

        _repo.Verify(r => r.AddAsync(
            It.Is<Ticket>(t => t.AccountId == accountId && t.ContactId == contactId && t.Priority == TicketPriority.High),
            default), Times.Once);
    }
}

[TestFixture]
public class CreateTicketValidatorTests
{
    private CreateTicketValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateTicketValidator();

    [Test]
    public async Task Validate_EmptySubject_Fails()
    {
        var result = await _validator.ValidateAsync(new CreateTicketCommand(""));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Subject");
    }

    [Test]
    public async Task Validate_ValidCommand_Passes()
    {
        var result = await _validator.ValidateAsync(new CreateTicketCommand("Server down"));
        result.IsValid.Should().BeTrue();
    }
}
