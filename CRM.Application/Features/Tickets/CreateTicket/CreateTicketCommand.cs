using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Tickets.CreateTicket;

public class CreateTicketCommand : IRequest<Guid>
{
    public CreateTicketCommand(string subject) => Subject = subject;
    public string Subject { get; init; }
    public Guid? AccountId { get; init; }
    public Guid? ContactId { get; init; }
    public TicketPriority Priority { get; init; } = TicketPriority.Medium;
    public TicketStatus Status { get; init; } = TicketStatus.Open;
}
