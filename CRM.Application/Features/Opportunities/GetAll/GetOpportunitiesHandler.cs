using CRM.Application.Common;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Opportunities.GetAll;

public class GetOpportunitiesHandler(IOpportunityRepository repo) : IRequestHandler<GetOpportunitiesQuery, PagedResult<OpportunitySummaryDto>>
{
    public async Task<PagedResult<OpportunitySummaryDto>> Handle(GetOpportunitiesQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repo.GetPagedAsync(request.Search, request.Page, request.PageSize, cancellationToken);

        var dtos = items.Select(o => new OpportunitySummaryDto(
            o.Id, o.Name, o.Stage, o.Amount, o.Probability, o.CloseDate, o.AccountId, o.CreatedAt
        )).ToList();

        return new PagedResult<OpportunitySummaryDto>(dtos, total, request.Page, request.PageSize);
    }
}
