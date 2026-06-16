using CRM.Application.Common;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Leads.GetAll;

public class GetLeadsHandler(ILeadRepository repo) : IRequestHandler<GetLeadsQuery, PagedResult<LeadSummaryDto>>
{
    public async Task<PagedResult<LeadSummaryDto>> Handle(GetLeadsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repo.GetPagedAsync(request.Search, request.Page, request.PageSize, cancellationToken);

        var dtos = items.Select(l => new LeadSummaryDto(
            l.Id, l.FirstName, l.LastName, l.Company, l.Email, l.Status, l.Source, l.CreatedAt
        )).ToList();

        return new PagedResult<LeadSummaryDto>(dtos, total, request.Page, request.PageSize);
    }
}
