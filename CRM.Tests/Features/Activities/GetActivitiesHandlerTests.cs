using CRM.Application.Exceptions;
using CRM.Application.Features.Activities.GetAll;
using CRM.Application.Features.Activities.GetById;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Activities;

[TestFixture]
public class GetActivitiesHandlerTests
{
    private Mock<IActivityRepository> _repo = null!;
    private GetActivitiesHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IActivityRepository>();
        _handler = new GetActivitiesHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ReturnsPagedResult()
    {
        var activities = new List<Activity>
        {
            new() { Id = Guid.NewGuid(), Title = "Follow-up call", ActivityType = ActivityType.Call, Status = ActivityStatus.NotStarted, Priority = Priority.High },
            new() { Id = Guid.NewGuid(), Title = "Send proposal",  ActivityType = ActivityType.Email, Status = ActivityStatus.InProgress, Priority = Priority.Medium },
        };
        _repo.Setup(r => r.GetPagedAsync("", 1, 10, default))
             .ReturnsAsync(((IReadOnlyList<Activity>)activities, 2));

        var result = await _handler.Handle(new GetActivitiesQuery(), default);

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().Title.Should().Be("Follow-up call");
    }

    [Test]
    public async Task Handle_SearchTerm_PassedToRepository()
    {
        _repo.Setup(r => r.GetPagedAsync("call", 1, 10, default))
             .ReturnsAsync(((IReadOnlyList<Activity>)new List<Activity>(), 0));

        await _handler.Handle(new GetActivitiesQuery { Search = "call" }, default);

        _repo.Verify(r => r.GetPagedAsync("call", 1, 10, default), Times.Once);
    }
}

[TestFixture]
public class GetActivityByIdHandlerTests
{
    private Mock<IActivityRepository> _repo = null!;
    private GetActivityByIdHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IActivityRepository>();
        _handler = new GetActivityByIdHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingId_ReturnsDto()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default))
             .ReturnsAsync(new Activity { Id = id, Title = "Follow-up call", ActivityType = ActivityType.Call });

        var result = await _handler.Handle(new GetActivityByIdQuery(id), default);

        result.Id.Should().Be(id);
        result.Title.Should().Be("Follow-up call");
        result.ActivityType.Should().Be(ActivityType.Call);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Activity?)null);

        var act = () => _handler.Handle(new GetActivityByIdQuery(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
