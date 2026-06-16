using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Accounts.GetById;

public class GetAccountByIdHandler(IAccountRepository repo) : IRequestHandler<GetAccountByIdQuery, AccountDetailDto>
{
    public async Task<AccountDetailDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Account {request.Id} not found.");

        return new AccountDetailDto(
            a.Id, a.Name, a.AccountType, a.Industry, a.Website,
            a.PhoneNumber, a.AnnualRevenue, a.EmployeeCount,
            a.BillingAddress, a.Description, a.CreatedAt, a.UpdatedAt
        );
    }
}
