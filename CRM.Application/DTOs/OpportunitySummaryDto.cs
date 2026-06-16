using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record OpportunitySummaryDto(
    Guid Id,
    string Name,
    OpportunityStage Stage,
    decimal Amount,
    int Probability,
    DateTime? CloseDate,
    Guid? AccountId,
    DateTime CreatedAt);
