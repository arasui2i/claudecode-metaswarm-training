using CRM.Application.Features.Customers.GetAll;
using CRM.Application.Features.Customers.GetById;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Customers;

[TestFixture]
public class GetCustomersHandlerTests
{
    private Mock<ICustomerRepository> _repo = null!;
    private GetCustomersHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ICustomerRepository>();
        _handler = new GetCustomersHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ReturnsPagedResult()
    {
        var customers = new List<Customer>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Smith", Email = "alice@crm.com", Status = CustomerStatus.Active },
            new() { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Jones", Email = "bob@crm.com", Status = CustomerStatus.Lead },
        };
        _repo.Setup(r => r.GetPagedAsync("", 1, 10, default)).ReturnsAsync((customers, 2));

        var result = await _handler.Handle(new GetCustomersQuery(), default);

        result.Total.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.First().FirstName.Should().Be("Alice");
    }

    [Test]
    public async Task Handle_SearchTerm_PassedToRepository()
    {
        _repo.Setup(r => r.GetPagedAsync("alice", 1, 10, default)).ReturnsAsync((new List<Customer>(), 0));

        await _handler.Handle(new GetCustomersQuery { Search = "alice" }, default);

        _repo.Verify(r => r.GetPagedAsync("alice", 1, 10, default), Times.Once);
    }
}

[TestFixture]
public class GetCustomerByIdHandlerTests
{
    private Mock<ICustomerRepository> _repo = null!;
    private GetCustomerByIdHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ICustomerRepository>();
        _handler = new GetCustomerByIdHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingId_ReturnsDto()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default))
             .ReturnsAsync(new Customer { Id = id, FirstName = "Alice", LastName = "Smith", Email = "alice@crm.com" });

        var result = await _handler.Handle(new GetCustomerByIdQuery(id), default);

        result.Id.Should().Be(id);
        result.FirstName.Should().Be("Alice");
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Customer?)null);

        var act = () => _handler.Handle(new GetCustomerByIdQuery(id), default);
        await act.Should().ThrowAsync<CRM.Application.Exceptions.NotFoundException>();
    }
}
