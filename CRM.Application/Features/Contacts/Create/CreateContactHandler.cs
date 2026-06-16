using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using MediatR;

namespace CRM.Application.Features.Contacts.Create;

public class CreateContactHandler(IContactRepository repo) : IRequestHandler<CreateContactCommand, Guid>
{
    public async Task<Guid> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        if (await repo.EmailExistsAsync(request.Email, null, cancellationToken))
            throw new InvalidOperationException("A contact with this email already exists.");

        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            JobTitle = request.JobTitle,
            Department = request.Department,
            ContactType = request.ContactType,
            CustomerId = request.CustomerId,
            Notes = request.Notes,
        };

        await repo.AddAsync(contact, cancellationToken);
        return contact.Id;
    }
}
