using CRM.Application.Exceptions;
using CRM.Application.Features.Accounts.Delete;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Accounts;

[TestFixture]
public class DeleteAccountHandlerTests
{
    private Mock<IAccountRepository> _repo = null!;
    private DeleteAccountHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IAccountRepository>();
        _handler = new DeleteAccountHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingAccount_SetsIsDeletedAndUpdatedAt()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Account { Id = id });
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Account>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteAccountCommand(id), default);

        _repo.Verify(r => r.UpdateAsync(
            It.Is<Account>(a => a.IsDeleted && a.UpdatedAt != default), default), Times.Once);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Account?)null);

        var act = () => _handler.Handle(new DeleteAccountCommand(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
