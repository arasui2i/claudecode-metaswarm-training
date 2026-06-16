using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Contacts.GetById;

public class GetContactByIdHandler(IContactRepository repo) : IRequestHandler<GetContactByIdQuery, ContactDetailDto>
{
    public async Task<ContactDetailDto> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var c = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contact {request.Id} not found.");

        return new ContactDetailDto(
            c.Id, c.FirstName, c.LastName, c.Email, c.PhoneNumber,
            c.JobTitle, c.Department, c.ContactType, c.CustomerId,
            c.Notes, c.CreatedAt, c.UpdatedAt
        );
    }
}
