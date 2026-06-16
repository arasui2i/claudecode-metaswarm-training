using CRM.Application.Exceptions;
using CRM.Application.Features.Tickets.DeleteTicket;
using CRM.Application.Features.Tickets.GetTicketById;
using CRM.Application.Features.Tickets.GetTickets;
using CRM.Application.Features.Tickets.UpdateTicket;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Tickets;

[TestFixture]
public class GetTicketsHandlerTests
{
    private Mock<ITicketRepository> _repo = null!;
    private GetTicketsHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ITicketRepository>();
        _handler = new GetTicketsHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ReturnsPagedResult()
    {
        var tickets = new List<Ticket>
        {
            new() { Id = Guid.NewGuid(), TicketNumber = "TKT-00001", Subject = "Server down", Priority = TicketPriority.High, Status = TicketStatus.Open },
            new() { Id = Guid.NewGuid(), TicketNumber = "TKT-00002", Subject = "Login issue", Priority = TicketPriority.Medium, Status = TicketStatus.InProgress },
        };
        _repo.Setup(r => r.GetPagedAsync("", 1, 10, default))
             .ReturnsAsync(((IReadOnlyList<Ticket>)tickets, 2));

        var result = await _handler.Handle(new GetTicketsQuery(), default);

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().TicketNumber.Should().Be("TKT-00001");
    }

    [Test]
    public async Task Handle_SearchTerm_PassedToRepository()
    {
        _repo.Setup(r => r.GetPagedAsync("server", 1, 10, default))
             .ReturnsAsync(((IReadOnlyList<Ticket>)new List<Ticket>(), 0));

        await _handler.Handle(new GetTicketsQuery { Search = "server" }, default);

        _repo.Verify(r => r.GetPagedAsync("server", 1, 10, default), Times.Once);
    }
}

[TestFixture]
public class GetTicketByIdHandlerTests
{
    private Mock<ITicketRepository> _repo = null!;
    private GetTicketByIdHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ITicketRepository>();
        _handler = new GetTicketByIdHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingId_ReturnsDto()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default))
             .ReturnsAsync(new Ticket { Id = id, TicketNumber = "TKT-00001", Subject = "Server down", Priority = TicketPriority.High });

        var result = await _handler.Handle(new GetTicketByIdQuery(id), default);

        result.Id.Should().Be(id);
        result.TicketNumber.Should().Be("TKT-00001");
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Ticket?)null);

        var act = () => _handler.Handle(new GetTicketByIdQuery(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}

[TestFixture]
public class UpdateTicketHandlerTests
{
    private Mock<ITicketRepository> _repo = null!;
    private UpdateTicketHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ITicketRepository>();
        _handler = new UpdateTicketHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidUpdate_SavesChanges()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Ticket { Id = id, Subject = "Old" });
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Ticket>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new UpdateTicketCommand(id, "New subject") { Status = TicketStatus.InProgress }, default);

        _repo.Verify(r => r.UpdateAsync(
            It.Is<Ticket>(t => t.Subject == "New subject" && t.Status == TicketStatus.InProgress), default), Times.Once);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Ticket?)null);

        var act = () => _handler.Handle(new UpdateTicketCommand(id, "Subject"), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}

[TestFixture]
public class DeleteTicketHandlerTests
{
    private Mock<ITicketRepository> _repo = null!;
    private DeleteTicketHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ITicketRepository>();
        _handler = new DeleteTicketHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingTicket_CallsSoftDelete()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Ticket { Id = id });
        _repo.Setup(r => r.SoftDeleteAsync(id, default)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteTicketCommand(id), default);

        _repo.Verify(r => r.SoftDeleteAsync(id, default), Times.Once);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Ticket?)null);

        var act = () => _handler.Handle(new DeleteTicketCommand(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
