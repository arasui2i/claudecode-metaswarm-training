using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Tickets.UpdateTicket;

public class UpdateTicketCommand : IRequest
{
    public UpdateTicketCommand(Guid id, string subject) { Id = id; Subject = subject; }
    public Guid Id { get; init; }
    public string Subject { get; init; }
    public Guid? AccountId { get; init; }
    public Guid? ContactId { get; init; }
    public TicketPriority Priority { get; init; } = TicketPriority.Medium;
    public TicketStatus Status { get; init; } = TicketStatus.Open;
}
