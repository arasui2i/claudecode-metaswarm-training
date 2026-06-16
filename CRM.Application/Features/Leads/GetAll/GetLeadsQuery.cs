using CRM.Application.Common;
using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Leads.GetAll;

public record GetLeadsQuery : IRequest<PagedResult<LeadSummaryDto>>
{
    public string Search { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
