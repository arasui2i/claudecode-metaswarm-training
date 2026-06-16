using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record CustomerSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Company,
    string Email,
    CustomerStatus Status,
    string JobTitle,
    DateTime CreatedAt);
