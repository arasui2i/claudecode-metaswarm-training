using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public Guid? AccountId { get; set; }
    public Guid? ContactId { get; set; }
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Account? Account { get; set; }
    public Contact? Contact { get; set; }
}
