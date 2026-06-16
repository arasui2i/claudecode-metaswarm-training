using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Accounts.Update;

public class UpdateAccountCommand : IRequest
{
    public UpdateAccountCommand(Guid id, string name) { Id = id; Name = name; }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public AccountType AccountType { get; init; } = AccountType.Prospect;
    public string Industry { get; init; } = string.Empty;
    public string Website { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public decimal AnnualRevenue { get; init; }
    public int EmployeeCount { get; init; }
    public string BillingAddress { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
