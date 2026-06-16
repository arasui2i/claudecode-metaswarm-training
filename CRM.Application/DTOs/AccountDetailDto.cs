using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record AccountDetailDto(
    Guid Id,
    string Name,
    AccountType AccountType,
    string Industry,
    string Website,
    string PhoneNumber,
    decimal AnnualRevenue,
    int EmployeeCount,
    string BillingAddress,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);
