using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record ContactDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string JobTitle,
    string Department,
    ContactType ContactType,
    Guid? CustomerId,
    string Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt);
