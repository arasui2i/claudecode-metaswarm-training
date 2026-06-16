using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Contacts.Update;

public class UpdateContactHandler(IContactRepository repo) : IRequestHandler<UpdateContactCommand>
{
    public async Task Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contact {request.Id} not found.");

        if (await repo.EmailExistsAsync(request.Email, request.Id, cancellationToken))
            throw new InvalidOperationException("A contact with this email already exists.");

        contact.FirstName = request.FirstName;
        contact.LastName = request.LastName;
        contact.Email = request.Email;
        contact.PhoneNumber = request.PhoneNumber;
        contact.JobTitle = request.JobTitle;
        contact.Department = request.Department;
        contact.ContactType = request.ContactType;
        contact.CustomerId = request.CustomerId;
        contact.Notes = request.Notes;
        contact.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(contact, cancellationToken);
    }
}
