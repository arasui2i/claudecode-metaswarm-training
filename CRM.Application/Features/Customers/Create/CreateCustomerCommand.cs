using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Customers.Create;

public class CreateCustomerCommand : IRequest<Guid>
{
    public CreateCustomerCommand(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Company { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public CustomerStatus Status { get; init; } = CustomerStatus.Active;
    public string JobTitle { get; init; } = string.Empty;
    public Gender Gender { get; init; } = Gender.Other;
    public int Age { get; init; }
    public string Industry { get; init; } = string.Empty;
    public decimal AnnualIncome { get; init; }
    public int EmployeeCount { get; init; }
    public string HeadquartersAddress { get; init; } = string.Empty;
}
