using CRM.Application.Common;
using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Customers.GetAll;

public record GetCustomersQuery : IRequest<PagedResult<CustomerSummaryDto>>
{
    public string Search { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
