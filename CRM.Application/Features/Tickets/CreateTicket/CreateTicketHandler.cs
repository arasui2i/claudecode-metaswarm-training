using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using MediatR;

namespace CRM.Application.Features.Tickets.CreateTicket;

public class CreateTicketHandler(ITicketRepository repo) : IRequestHandler<CreateTicketCommand, Guid>
{
    public async Task<Guid> Handle(CreateTicketCommand request, CancellationToken ct)
    {
        var ticketNumber = await repo.GetNextTicketNumberAsync(ct);
        var ticket = new Ticket
        {
            TicketNumber = ticketNumber,
            Subject = request.Subject,
            AccountId = request.AccountId,
            ContactId = request.ContactId,
            Priority = request.Priority,
            Status = request.Status,
        };
        await repo.AddAsync(ticket, ct);
        return ticket.Id;
    }
}
