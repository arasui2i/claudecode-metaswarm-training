using CRM.Application.Exceptions;
using CRM.Application.Features.Customers.Update;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Customers;

[TestFixture]
public class UpdateCustomerHandlerTests
{
    private Mock<ICustomerRepository> _repo = null!;
    private UpdateCustomerHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ICustomerRepository>();
        _handler = new UpdateCustomerHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidUpdate_SavesChanges()
    {
        var id = Guid.NewGuid();
        var existing = new Customer { Id = id, FirstName = "Old", LastName = "Name", Email = "old@crm.com" };
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(existing);
        _repo.Setup(r => r.EmailExistsAsync("new@crm.com", id, default)).ReturnsAsync(false);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Customer>(), default)).Returns(Task.CompletedTask);

        var cmd = new UpdateCustomerCommand(id, "New", "Name", "new@crm.com");
        await _handler.Handle(cmd, default);

        _repo.Verify(r => r.UpdateAsync(It.Is<Customer>(c =>
            c.FirstName == "New" && c.Email == "new@crm.com"), default), Times.Once);
    }

    [Test]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(
            new Customer { Id = id, FirstName = "X", LastName = "Y", Email = "x@crm.com" });
        _repo.Setup(r => r.EmailExistsAsync("taken@crm.com", id, default)).ReturnsAsync(true);

        var cmd = new UpdateCustomerCommand(id, "X", "Y", "taken@crm.com");
        var act = () => _handler.Handle(cmd, default);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Customer?)null);

        var act = () => _handler.Handle(new UpdateCustomerCommand(id, "A", "B", "a@b.com"), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
