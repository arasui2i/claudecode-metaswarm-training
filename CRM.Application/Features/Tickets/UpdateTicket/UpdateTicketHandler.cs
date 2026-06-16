using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Tickets.UpdateTicket;

public class UpdateTicketHandler(ITicketRepository repo) : IRequestHandler<UpdateTicketCommand>
{
    public async Task Handle(UpdateTicketCommand request, CancellationToken ct)
    {
        var ticket = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"Ticket {request.Id} not found.");

        ticket.Subject = request.Subject;
        ticket.AccountId = request.AccountId;
        ticket.ContactId = request.ContactId;
        ticket.Priority = request.Priority;
        ticket.Status = request.Status;
        ticket.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(ticket, ct);
    }
}
