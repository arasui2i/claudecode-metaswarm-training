using CRM.Application.Features.Activities.Create;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Activities;

[TestFixture]
public class CreateActivityHandlerTests
{
    private Mock<IActivityRepository> _repo = null!;
    private CreateActivityHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IActivityRepository>();
        _handler = new CreateActivityHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_CreatesActivityAndReturnsId()
    {
        _repo.Setup(r => r.AddAsync(It.IsAny<Activity>(), default)).Returns(Task.CompletedTask);

        var id = await _handler.Handle(new CreateActivityCommand("Follow-up call"), default);

        id.Should().NotBeEmpty();
        _repo.Verify(r => r.AddAsync(It.Is<Activity>(a => a.Title == "Follow-up call"), default), Times.Once);
    }

    [Test]
    public async Task Handle_WithRelatedEntity_SetsRelationship()
    {
        var relatedId = Guid.NewGuid();
        _repo.Setup(r => r.AddAsync(It.IsAny<Activity>(), default)).Returns(Task.CompletedTask);

        var cmd = new CreateActivityCommand("Site visit") { RelatedEntityType = "Customer", RelatedEntityId = relatedId };
        await _handler.Handle(cmd, default);

        _repo.Verify(r => r.AddAsync(It.Is<Activity>(a =>
            a.RelatedEntityType == "Customer" && a.RelatedEntityId == relatedId), default), Times.Once);
    }
}

[TestFixture]
public class CreateActivityValidatorTests
{
    private CreateActivityValidator _validator = null!;

    [SetUp]
    public void SetUp() => _validator = new CreateActivityValidator();

    [Test]
    public async Task Validate_EmptyTitle_Fails()
    {
        var result = await _validator.ValidateAsync(new CreateActivityCommand(""));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Test]
    public async Task Validate_ValidCommand_Passes()
    {
        var result = await _validator.ValidateAsync(new CreateActivityCommand("Follow-up call"));
        result.IsValid.Should().BeTrue();
    }
}
