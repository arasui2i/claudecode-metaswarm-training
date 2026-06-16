using CRM.Application.Exceptions;
using CRM.Application.Features.Customers.Delete;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Customers;

[TestFixture]
public class DeleteCustomerHandlerTests
{
    private Mock<ICustomerRepository> _repo = null!;
    private DeleteCustomerHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ICustomerRepository>();
        _handler = new DeleteCustomerHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingCustomer_SetsIsDeletedAndUpdatedAt()
    {
        var id = Guid.NewGuid();
        var customer = new Customer { Id = id, IsDeleted = false };
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(customer);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Customer>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteCustomerCommand(id), default);

        _repo.Verify(r => r.UpdateAsync(It.Is<Customer>(c => c.IsDeleted && c.UpdatedAt != default), default), Times.Once);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Customer?)null);

        var act = () => _handler.Handle(new DeleteCustomerCommand(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
