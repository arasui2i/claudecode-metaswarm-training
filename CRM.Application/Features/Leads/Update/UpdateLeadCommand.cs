using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Leads.Update;

public class UpdateLeadCommand : IRequest
{
    public UpdateLeadCommand(Guid id, string firstName, string lastName, string email)
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
    public string Company { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public LeadStatus Status { get; init; } = LeadStatus.New;
    public LeadSource Source { get; init; } = LeadSource.Other;
    public string Notes { get; init; } = string.Empty;
}
