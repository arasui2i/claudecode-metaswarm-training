using CRM.Application.Exceptions;
using CRM.Application.Features.Opportunities.GetAll;
using CRM.Application.Features.Opportunities.GetById;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Opportunities;

[TestFixture]
public class GetOpportunitiesHandlerTests
{
    private Mock<IOpportunityRepository> _repo = null!;
    private GetOpportunitiesHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IOpportunityRepository>();
        _handler = new GetOpportunitiesHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ReturnsPagedResult()
    {
        var opps = new List<Opportunity>
        {
            new() { Id = Guid.NewGuid(), Name = "Deal A", Stage = OpportunityStage.Proposal, Amount = 10000m },
            new() { Id = Guid.NewGuid(), Name = "Deal B", Stage = OpportunityStage.ClosedWon, Amount = 25000m },
        };
        _repo.Setup(r => r.GetPagedAsync("", 1, 10, default))
             .ReturnsAsync(((IReadOnlyList<Opportunity>)opps, 2));

        var result = await _handler.Handle(new GetOpportunitiesQuery(), default);

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().Name.Should().Be("Deal A");
    }

    [Test]
    public async Task Handle_SearchTerm_PassedToRepository()
    {
        _repo.Setup(r => r.GetPagedAsync("deal", 1, 10, default))
             .ReturnsAsync(((IReadOnlyList<Opportunity>)new List<Opportunity>(), 0));

        await _handler.Handle(new GetOpportunitiesQuery { Search = "deal" }, default);

        _repo.Verify(r => r.GetPagedAsync("deal", 1, 10, default), Times.Once);
    }
}

[TestFixture]
public class GetOpportunityByIdHandlerTests
{
    private Mock<IOpportunityRepository> _repo = null!;
    private GetOpportunityByIdHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IOpportunityRepository>();
        _handler = new GetOpportunityByIdHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingId_ReturnsDto()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default))
             .ReturnsAsync(new Opportunity { Id = id, Name = "Deal A", Amount = 10000m });

        var result = await _handler.Handle(new GetOpportunityByIdQuery(id), default);

        result.Id.Should().Be(id);
        result.Name.Should().Be("Deal A");
        result.Amount.Should().Be(10000m);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Opportunity?)null);

        var act = () => _handler.Handle(new GetOpportunityByIdQuery(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
