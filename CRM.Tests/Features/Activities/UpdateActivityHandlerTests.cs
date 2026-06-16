using CRM.Application.Exceptions;
using CRM.Application.Features.Activities.Update;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Activities;

[TestFixture]
public class UpdateActivityHandlerTests
{
    private Mock<IActivityRepository> _repo = null!;
    private UpdateActivityHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IActivityRepository>();
        _handler = new UpdateActivityHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidUpdate_SavesChanges()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Activity { Id = id, Title = "Old title" });
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Activity>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new UpdateActivityCommand(id, "New title") { Status = ActivityStatus.Completed }, default);

        _repo.Verify(r => r.UpdateAsync(It.Is<Activity>(a =>
            a.Title == "New title" && a.Status == ActivityStatus.Completed), default), Times.Once);
    }

    [Test]
    public async Task Handle_CompletedStatus_SetsCompletedAt()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Activity { Id = id, Title = "Task" });
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Activity>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new UpdateActivityCommand(id, "Task") { Status = ActivityStatus.Completed }, default);

        _repo.Verify(r => r.UpdateAsync(It.Is<Activity>(a => a.CompletedAt.HasValue), default), Times.Once);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Activity?)null);

        var act = () => _handler.Handle(new UpdateActivityCommand(id, "Title"), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
