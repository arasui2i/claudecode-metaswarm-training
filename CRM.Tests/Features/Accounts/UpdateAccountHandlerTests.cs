using CRM.Application.Exceptions;
using CRM.Application.Features.Accounts.Update;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Accounts;

[TestFixture]
public class UpdateAccountHandlerTests
{
    private Mock<IAccountRepository> _repo = null!;
    private UpdateAccountHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IAccountRepository>();
        _handler = new UpdateAccountHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidUpdate_SavesChanges()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Account { Id = id, Name = "Old Corp" });
        _repo.Setup(r => r.NameExistsAsync("New Corp", id, default)).ReturnsAsync(false);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Account>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new UpdateAccountCommand(id, "New Corp"), default);

        _repo.Verify(r => r.UpdateAsync(It.Is<Account>(a => a.Name == "New Corp"), default), Times.Once);
    }

    [Test]
    public async Task Handle_DuplicateName_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Account { Id = id, Name = "Old Corp" });
        _repo.Setup(r => r.NameExistsAsync("Taken Corp", id, default)).ReturnsAsync(true);

        var act = () => _handler.Handle(new UpdateAccountCommand(id, "Taken Corp"), default);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Account?)null);

        var act = () => _handler.Handle(new UpdateAccountCommand(id, "Any Corp"), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
