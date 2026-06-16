using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record ActivitySummaryDto(
    Guid Id,
    string Title,
    ActivityType ActivityType,
    ActivityStatus Status,
    Priority Priority,
    DateTime? DueDate,
    string? RelatedEntityType,
    Guid? RelatedEntityId,
    DateTime CreatedAt);
