using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Accounts.Update;

public class UpdateAccountHandler(IAccountRepository repo) : IRequestHandler<UpdateAccountCommand>
{
    public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Account {request.Id} not found.");

        if (await repo.NameExistsAsync(request.Name, request.Id, cancellationToken))
            throw new InvalidOperationException("An account with this name already exists.");

        account.Name = request.Name;
        account.AccountType = request.AccountType;
        account.Industry = request.Industry;
        account.Website = request.Website;
        account.PhoneNumber = request.PhoneNumber;
        account.AnnualRevenue = request.AnnualRevenue;
        account.EmployeeCount = request.EmployeeCount;
        account.BillingAddress = request.BillingAddress;
        account.Description = request.Description;
        account.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(account, cancellationToken);
    }
}
