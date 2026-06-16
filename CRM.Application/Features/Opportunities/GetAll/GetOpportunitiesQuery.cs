using CRM.Application.Common;
using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Opportunities.GetAll;

public record GetOpportunitiesQuery : IRequest<PagedResult<OpportunitySummaryDto>>
{
    public string Search { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
