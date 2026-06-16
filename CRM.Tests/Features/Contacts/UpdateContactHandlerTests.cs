using CRM.Application.Exceptions;
using CRM.Application.Features.Contacts.Update;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Contacts;

[TestFixture]
public class UpdateContactHandlerTests
{
    private Mock<IContactRepository> _repo = null!;
    private UpdateContactHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IContactRepository>();
        _handler = new UpdateContactHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ValidUpdate_SavesChanges()
    {
        var id = Guid.NewGuid();
        var existing = new Contact { Id = id, FirstName = "Old", LastName = "Name", Email = "old@crm.com" };
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(existing);
        _repo.Setup(r => r.EmailExistsAsync("new@crm.com", id, default)).ReturnsAsync(false);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Contact>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new UpdateContactCommand(id, "New", "Name", "new@crm.com"), default);

        _repo.Verify(r => r.UpdateAsync(It.Is<Contact>(c =>
            c.FirstName == "New" && c.Email == "new@crm.com"), default), Times.Once);
    }

    [Test]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(
            new Contact { Id = id, FirstName = "X", LastName = "Y", Email = "x@crm.com" });
        _repo.Setup(r => r.EmailExistsAsync("taken@crm.com", id, default)).ReturnsAsync(true);

        var act = () => _handler.Handle(new UpdateContactCommand(id, "X", "Y", "taken@crm.com"), default);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Contact?)null);

        var act = () => _handler.Handle(new UpdateContactCommand(id, "A", "B", "a@b.com"), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
