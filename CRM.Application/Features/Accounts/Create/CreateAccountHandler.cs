using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using MediatR;

namespace CRM.Application.Features.Accounts.Create;

public class CreateAccountHandler(IAccountRepository repo) : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (await repo.NameExistsAsync(request.Name, null, cancellationToken))
            throw new InvalidOperationException("An account with this name already exists.");

        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            AccountType = request.AccountType,
            Industry = request.Industry,
            Website = request.Website,
            PhoneNumber = request.PhoneNumber,
            AnnualRevenue = request.AnnualRevenue,
            EmployeeCount = request.EmployeeCount,
            BillingAddress = request.BillingAddress,
            Description = request.Description,
        };

        await repo.AddAsync(account, cancellationToken);
        return account.Id;
    }
}
