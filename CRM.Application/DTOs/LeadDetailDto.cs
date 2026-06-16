using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record LeadDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Company,
    string JobTitle,
    LeadStatus Status,
    LeadSource Source,
    string Notes,
    Guid? ConvertedCustomerId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
