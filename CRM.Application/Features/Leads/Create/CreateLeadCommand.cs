using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Leads.Create;

public class CreateLeadCommand : IRequest<Guid>
{
    public CreateLeadCommand(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public LeadStatus Status { get; init; } = LeadStatus.New;
    public LeadSource Source { get; init; } = LeadSource.Other;
    public string Notes { get; init; } = string.Empty;
}
