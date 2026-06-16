using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Contacts.Update;

public class UpdateContactCommand : IRequest
{
    public UpdateContactCommand(Guid id, string firstName, string lastName, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public ContactType ContactType { get; init; } = ContactType.Other;
    public Guid? CustomerId { get; init; }
    public string Notes { get; init; } = string.Empty;
}
