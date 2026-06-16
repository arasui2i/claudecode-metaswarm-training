using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record ContactSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string JobTitle,
    ContactType ContactType,
    Guid? CustomerId,
    DateTime CreatedAt);
