using CRM.Application.Common;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Tickets.GetTickets;

public class GetTicketsHandler(ITicketRepository repo) : IRequestHandler<GetTicketsQuery, PagedResult<TicketSummaryDto>>
{
    public async Task<PagedResult<TicketSummaryDto>> Handle(GetTicketsQuery request, CancellationToken ct)
    {
        var (items, total) = await repo.GetPagedAsync(request.Search, request.Page, request.PageSize, ct);
        var dtos = items.Select(t => new TicketSummaryDto(t.Id, t.TicketNumber, t.Subject, t.Priority, t.Status)).ToList();
        return new PagedResult<TicketSummaryDto>(dtos, total, request.Page, request.PageSize);
    }
}
