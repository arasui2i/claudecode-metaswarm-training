using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record AccountSummaryDto(
    Guid Id,
    string Name,
    AccountType AccountType,
    string Industry,
    string Website,
    string PhoneNumber,
    int EmployeeCount,
    DateTime CreatedAt);
