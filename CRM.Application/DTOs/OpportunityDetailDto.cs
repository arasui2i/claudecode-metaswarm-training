using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record OpportunityDetailDto(
    Guid Id,
    string Name,
    OpportunityStage Stage,
    decimal Amount,
    int Probability,
    DateTime? CloseDate,
    string Description,
    Guid? AccountId,
    Guid? ContactId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
