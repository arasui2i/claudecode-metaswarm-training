using CRM.Application.Exceptions;
using CRM.Application.Features.Contacts.GetAll;
using CRM.Application.Features.Contacts.GetById;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Contacts;

[TestFixture]
public class GetContactsHandlerTests
{
    private Mock<IContactRepository> _repo = null!;
    private GetContactsHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IContactRepository>();
        _handler = new GetContactsHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ReturnsPagedResult()
    {
        var contacts = new List<Contact>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Smith", Email = "alice@crm.com", ContactType = ContactType.Primary },
            new() { Id = Guid.NewGuid(), FirstName = "Bob",   LastName = "Jones", Email = "bob@crm.com",   ContactType = ContactType.Secondary },
        };
        _repo.Setup(r => r.GetPagedAsync("", 1, 10, default)).ReturnsAsync(((IReadOnlyList<Contact>)contacts, 2));

        var result = await _handler.Handle(new GetContactsQuery(), default);

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().FirstName.Should().Be("Alice");
    }

    [Test]
    public async Task Handle_SearchTerm_PassedToRepository()
    {
        _repo.Setup(r => r.GetPagedAsync("alice", 1, 10, default))
             .ReturnsAsync(((IReadOnlyList<Contact>)new List<Contact>(), 0));

        await _handler.Handle(new GetContactsQuery { Search = "alice" }, default);

        _repo.Verify(r => r.GetPagedAsync("alice", 1, 10, default), Times.Once);
    }
}

[TestFixture]
public class GetContactByIdHandlerTests
{
    private Mock<IContactRepository> _repo = null!;
    private GetContactByIdHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IContactRepository>();
        _handler = new GetContactByIdHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingId_ReturnsDto()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default))
             .ReturnsAsync(new Contact { Id = id, FirstName = "Alice", LastName = "Smith", Email = "alice@crm.com" });

        var result = await _handler.Handle(new GetContactByIdQuery(id), default);

        result.Id.Should().Be(id);
        result.FirstName.Should().Be("Alice");
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Contact?)null);

        var act = () => _handler.Handle(new GetContactByIdQuery(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
