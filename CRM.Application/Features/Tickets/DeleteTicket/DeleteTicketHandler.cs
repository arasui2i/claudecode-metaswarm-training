using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Tickets.DeleteTicket;

public class DeleteTicketHandler(ITicketRepository repo) : IRequestHandler<DeleteTicketCommand>
{
    public async Task Handle(DeleteTicketCommand request, CancellationToken ct)
    {
        var ticket = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"Ticket {request.Id} not found.");
        await repo.SoftDeleteAsync(ticket.Id, ct);
    }
}
