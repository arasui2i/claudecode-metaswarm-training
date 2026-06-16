using CRM.Application.Exceptions;
using CRM.Application.Features.Accounts.GetAll;
using CRM.Application.Features.Accounts.GetById;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Accounts;

[TestFixture]
public class GetAccountsHandlerTests
{
    private Mock<IAccountRepository> _repo = null!;
    private GetAccountsHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IAccountRepository>();
        _handler = new GetAccountsHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ReturnsPagedResult()
    {
        var accounts = new List<Account>
        {
            new() { Id = Guid.NewGuid(), Name = "Acme Corp",  AccountType = AccountType.Customer },
            new() { Id = Guid.NewGuid(), Name = "Beta Inc",   AccountType = AccountType.Prospect },
        };
        _repo.Setup(r => r.GetPagedAsync("", 1, 10, default))
             .ReturnsAsync(((IReadOnlyList<Account>)accounts, 2));

        var result = await _handler.Handle(new GetAccountsQuery(), default);

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().Name.Should().Be("Acme Corp");
    }

    [Test]
    public async Task Handle_SearchTerm_PassedToRepository()
    {
        _repo.Setup(r => r.GetPagedAsync("acme", 1, 10, default))
             .ReturnsAsync(((IReadOnlyList<Account>)new List<Account>(), 0));

        await _handler.Handle(new GetAccountsQuery { Search = "acme" }, default);

        _repo.Verify(r => r.GetPagedAsync("acme", 1, 10, default), Times.Once);
    }
}

[TestFixture]
public class GetAccountByIdHandlerTests
{
    private Mock<IAccountRepository> _repo = null!;
    private GetAccountByIdHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IAccountRepository>();
        _handler = new GetAccountByIdHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingId_ReturnsDto()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default))
             .ReturnsAsync(new Account { Id = id, Name = "Acme Corp" });

        var result = await _handler.Handle(new GetAccountByIdQuery(id), default);

        result.Id.Should().Be(id);
        result.Name.Should().Be("Acme Corp");
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Account?)null);

        var act = () => _handler.Handle(new GetAccountByIdQuery(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
