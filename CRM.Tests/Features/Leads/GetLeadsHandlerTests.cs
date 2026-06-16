using CRM.Application.Exceptions;
using CRM.Application.Features.Leads.GetAll;
using CRM.Application.Features.Leads.GetById;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Leads;

[TestFixture]
public class GetLeadsHandlerTests
{
    private Mock<ILeadRepository> _repo = null!;
    private GetLeadsHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ILeadRepository>();
        _handler = new GetLeadsHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ReturnsPagedResult()
    {
        var leads = new List<Lead>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Smith", Email = "alice@crm.com", Status = LeadStatus.New },
            new() { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Jones", Email = "bob@crm.com", Status = LeadStatus.Contacted },
        };
        _repo.Setup(r => r.GetPagedAsync("", 1, 10, default)).ReturnsAsync(((IReadOnlyList<Lead>)leads, 2));

        var result = await _handler.Handle(new GetLeadsQuery(), default);

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().FirstName.Should().Be("Alice");
    }

    [Test]
    public async Task Handle_SearchTerm_PassedToRepository()
    {
        _repo.Setup(r => r.GetPagedAsync("alice", 1, 10, default)).ReturnsAsync(((IReadOnlyList<Lead>)new List<Lead>(), 0));

        await _handler.Handle(new GetLeadsQuery { Search = "alice" }, default);

        _repo.Verify(r => r.GetPagedAsync("alice", 1, 10, default), Times.Once);
    }
}

[TestFixture]
public class GetLeadByIdHandlerTests
{
    private Mock<ILeadRepository> _repo = null!;
    private GetLeadByIdHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ILeadRepository>();
        _handler = new GetLeadByIdHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingId_ReturnsDto()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default))
             .ReturnsAsync(new Lead { Id = id, FirstName = "Alice", LastName = "Smith", Email = "alice@crm.com" });

        var result = await _handler.Handle(new GetLeadByIdQuery(id), default);

        result.Id.Should().Be(id);
        result.FirstName.Should().Be("Alice");
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Lead?)null);

        var act = () => _handler.Handle(new GetLeadByIdQuery(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
