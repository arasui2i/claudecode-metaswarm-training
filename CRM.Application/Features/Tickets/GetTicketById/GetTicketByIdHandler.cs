using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Tickets.GetTicketById;

public class GetTicketByIdHandler(ITicketRepository repo) : IRequestHandler<GetTicketByIdQuery, TicketDetailDto>
{
    public async Task<TicketDetailDto> Handle(GetTicketByIdQuery request, CancellationToken ct)
    {
        var t = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"Ticket {request.Id} not found.");

        return new TicketDetailDto(
            t.Id, t.TicketNumber, t.Subject, t.Priority, t.Status,
            t.AccountId, t.Account?.Name,
            t.ContactId,
            t.Contact != null ? $"{t.Contact.FirstName} {t.Contact.LastName}" : null,
            t.CreatedAt, t.UpdatedAt);
    }
}
