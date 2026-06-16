using CRM.Application.Exceptions;
using CRM.Application.Features.Leads.Delete;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Leads;

[TestFixture]
public class DeleteLeadHandlerTests
{
    private Mock<ILeadRepository> _repo = null!;
    private DeleteLeadHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ILeadRepository>();
        _handler = new DeleteLeadHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingLead_SetsIsDeletedAndUpdatedAt()
    {
        var id = Guid.NewGuid();
        var lead = new Lead { Id = id, IsDeleted = false };
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(lead);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Lead>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteLeadCommand(id), default);

        _repo.Verify(r => r.UpdateAsync(It.Is<Lead>(l => l.IsDeleted && l.UpdatedAt != default), default), Times.Once);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Lead?)null);

        var act = () => _handler.Handle(new DeleteLeadCommand(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
