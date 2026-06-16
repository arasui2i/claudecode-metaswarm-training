using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Opportunity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public OpportunityStage Stage { get; set; } = OpportunityStage.Prospecting;
    public decimal Amount { get; set; }
    public int Probability { get; set; }
    public DateTime? CloseDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? AccountId { get; set; }
    public Guid? ContactId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}
