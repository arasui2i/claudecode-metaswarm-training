using CRM.Application.Exceptions;
using CRM.Application.Features.Opportunities.Update;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Opportunities;

[TestFixture]
public class UpdateOpportunityHandlerTests
{
    private Mock<IOpportunityRepository> _repo = null!;
    private UpdateOpportunityHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IOpportunityRepository>();
        _handler = new UpdateOpportunityHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidUpdate_SavesChanges()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Opportunity { Id = id, Name = "Old Deal" });
        _repo.Setup(r => r.NameExistsAsync("New Deal", id, default)).ReturnsAsync(false);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Opportunity>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new UpdateOpportunityCommand(id, "New Deal") { Stage = OpportunityStage.Proposal, Amount = 5000m }, default);

        _repo.Verify(r => r.UpdateAsync(It.Is<Opportunity>(o =>
            o.Name == "New Deal" && o.Stage == OpportunityStage.Proposal && o.Amount == 5000m), default), Times.Once);
    }

    [Test]
    public async Task Handle_DuplicateName_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Opportunity { Id = id, Name = "Old Deal" });
        _repo.Setup(r => r.NameExistsAsync("Taken Deal", id, default)).ReturnsAsync(true);

        var act = () => _handler.Handle(new UpdateOpportunityCommand(id, "Taken Deal"), default);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Opportunity?)null);

        var act = () => _handler.Handle(new UpdateOpportunityCommand(id, "Any Deal"), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
