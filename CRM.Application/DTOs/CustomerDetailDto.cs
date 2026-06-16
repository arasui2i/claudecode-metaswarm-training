using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record CustomerDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Company,
    CustomerStatus Status,
    string JobTitle,
    Gender Gender,
    int Age,
    string Email,
    string PhoneNumber,
    string Industry,
    decimal AnnualIncome,
    int EmployeeCount,
    string HeadquartersAddress,
    DateTime CreatedAt,
    DateTime UpdatedAt);
