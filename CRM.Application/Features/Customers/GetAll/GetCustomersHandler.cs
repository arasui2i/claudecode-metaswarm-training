using CRM.Application.Common;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Customers.GetAll;

public class GetCustomersHandler(ICustomerRepository repo) : IRequestHandler<GetCustomersQuery, PagedResult<CustomerSummaryDto>>
{
    public async Task<PagedResult<CustomerSummaryDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repo.GetPagedAsync(request.Search, request.Page, request.PageSize, cancellationToken);

        var dtos = items.Select(c => new CustomerSummaryDto(
            c.Id, c.FirstName, c.LastName, c.Company, c.Email, c.Status, c.JobTitle, c.CreatedAt
        )).ToList();

        return new PagedResult<CustomerSummaryDto>(dtos, total, request.Page, request.PageSize);
    }
}
