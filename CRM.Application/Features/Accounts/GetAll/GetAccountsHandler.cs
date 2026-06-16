using CRM.Application.Common;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Accounts.GetAll;

public class GetAccountsHandler(IAccountRepository repo) : IRequestHandler<GetAccountsQuery, PagedResult<AccountSummaryDto>>
{
    public async Task<PagedResult<AccountSummaryDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repo.GetPagedAsync(request.Search, request.Page, request.PageSize, cancellationToken);

        var dtos = items.Select(a => new AccountSummaryDto(
            a.Id, a.Name, a.AccountType, a.Industry, a.Website,
            a.PhoneNumber, a.EmployeeCount, a.CreatedAt
        )).ToList();

        return new PagedResult<AccountSummaryDto>(dtos, total, request.Page, request.PageSize);
    }
}
