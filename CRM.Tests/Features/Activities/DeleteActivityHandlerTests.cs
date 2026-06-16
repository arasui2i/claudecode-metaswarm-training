using CRM.Application.Exceptions;
using CRM.Application.Features.Activities.Delete;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Activities;

[TestFixture]
public class DeleteActivityHandlerTests
{
    private Mock<IActivityRepository> _repo = null!;
    private DeleteActivityHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IActivityRepository>();
        _handler = new DeleteActivityHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingActivity_SetsIsDeletedAndUpdatedAt()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Activity { Id = id });
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Activity>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteActivityCommand(id), default);

        _repo.Verify(r => r.UpdateAsync(
            It.Is<Activity>(a => a.IsDeleted && a.UpdatedAt != default), default), Times.Once);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Activity?)null);

        var act = () => _handler.Handle(new DeleteActivityCommand(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
