using CRM.Application.Common;
using MediatR;

namespace CRM.Application.Features.Tickets.GetTickets;

public record GetTicketsQuery : IRequest<PagedResult<TicketSummaryDto>>
{
    public string Search { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
