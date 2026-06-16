using CRM.Application.Exceptions;
using CRM.Application.Features.Opportunities.Delete;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Opportunities;

[TestFixture]
public class DeleteOpportunityHandlerTests
{
    private Mock<IOpportunityRepository> _repo = null!;
    private DeleteOpportunityHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IOpportunityRepository>();
        _handler = new DeleteOpportunityHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingOpportunity_SetsIsDeletedAndUpdatedAt()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Opportunity { Id = id });
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Opportunity>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteOpportunityCommand(id), default);

        _repo.Verify(r => r.UpdateAsync(
            It.Is<Opportunity>(o => o.IsDeleted && o.UpdatedAt != default), default), Times.Once);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Opportunity?)null);

        var act = () => _handler.Handle(new DeleteOpportunityCommand(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
