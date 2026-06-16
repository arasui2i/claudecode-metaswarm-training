using CRM.Application.Features.Opportunities.Create;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Opportunities;

[TestFixture]
public class CreateOpportunityHandlerTests
{
    private Mock<IOpportunityRepository> _repo = null!;
    private CreateOpportunityHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IOpportunityRepository>();
        _handler = new CreateOpportunityHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_CreatesOpportunityAndReturnsId()
    {
        _repo.Setup(r => r.NameExistsAsync("Big Deal", null, default)).ReturnsAsync(false);
        _repo.Setup(r => r.AddAsync(It.IsAny<Opportunity>(), default)).Returns(Task.CompletedTask);

        var id = await _handler.Handle(new CreateOpportunityCommand("Big Deal"), default);

        id.Should().NotBeEmpty();
        _repo.Verify(r => r.AddAsync(It.Is<Opportunity>(o => o.Name == "Big Deal"), default), Times.Once);
    }

    [Test]
    public async Task Handle_DuplicateName_ThrowsInvalidOperationException()
    {
        _repo.Setup(r => r.NameExistsAsync("Dup Deal", null, default)).ReturnsAsync(true);

        var act = () => _handler.Handle(new CreateOpportunityCommand("Dup Deal"), default);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*name*");
    }
}

[TestFixture]
public class CreateOpportunityValidatorTests
{
    private CreateOpportunityValidator _validator = null!;
    private Mock<IOpportunityRepository> _repo = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IOpportunityRepository>();
        _validator = new CreateOpportunityValidator(_repo.Object);
    }

    [Test]
    public async Task Validate_EmptyName_Fails()
    {
        _repo.Setup(r => r.NameExistsAsync(It.IsAny<string>(), null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateOpportunityCommand(""));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Test]
    public async Task Validate_ProbabilityOutOfRange_Fails()
    {
        _repo.Setup(r => r.NameExistsAsync(It.IsAny<string>(), null, default)).ReturnsAsync(false);
        var cmd = new CreateOpportunityCommand("Deal") { Probability = 150 };
        var result = await _validator.ValidateAsync(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Probability");
    }

    [Test]
    public async Task Validate_ValidCommand_Passes()
    {
        _repo.Setup(r => r.NameExistsAsync("Good Deal", null, default)).ReturnsAsync(false);
        var result = await _validator.ValidateAsync(new CreateOpportunityCommand("Good Deal") { Probability = 75 });
        result.IsValid.Should().BeTrue();
    }
}
