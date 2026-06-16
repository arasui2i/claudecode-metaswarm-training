using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Contacts.Delete;

public class DeleteContactHandler(IContactRepository repo) : IRequestHandler<DeleteContactCommand>
{
    public async Task Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contact {request.Id} not found.");

        contact.IsDeleted = true;
        contact.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(contact, cancellationToken);
    }
}
