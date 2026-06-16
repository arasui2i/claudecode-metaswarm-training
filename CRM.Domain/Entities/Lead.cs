using CRM.Domain.Enums;

namespace CRM.Domain.Entities;

public class Lead
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public LeadSource Source { get; set; } = LeadSource.Other;
    public string Notes { get; set; } = string.Empty;
    public Guid? ConvertedCustomerId { get; set; }
    public DateTime? ConvertedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}
