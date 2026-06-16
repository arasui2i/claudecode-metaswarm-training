using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record LeadSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Company,
    string Email,
    LeadStatus Status,
    LeadSource Source,
    DateTime CreatedAt);
