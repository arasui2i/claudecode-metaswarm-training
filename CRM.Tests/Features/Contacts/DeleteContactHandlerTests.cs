using CRM.Application.Exceptions;
using CRM.Application.Features.Contacts.Delete;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Contacts;

[TestFixture]
public class DeleteContactHandlerTests
{
    private Mock<IContactRepository> _repo = null!;
    private DeleteContactHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IContactRepository>();
        _handler = new DeleteContactHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ExistingContact_SetsIsDeletedAndUpdatedAt()
    {
        var id = Guid.NewGuid();
        var contact = new Contact { Id = id, IsDeleted = false };
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(contact);
        _repo.Setup(r => r.UpdateAsync(It.IsAny<Contact>(), default)).Returns(Task.CompletedTask);

        await _handler.Handle(new DeleteContactCommand(id), default);

        _repo.Verify(r => r.UpdateAsync(
            It.Is<Contact>(c => c.IsDeleted && c.UpdatedAt != default), default), Times.Once);
    }

    [Test]
    public async Task Handle_UnknownId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync((Contact?)null);

        var act = () => _handler.Handle(new DeleteContactCommand(id), default);
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
