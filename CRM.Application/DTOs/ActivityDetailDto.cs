using CRM.Domain.Enums;

namespace CRM.Application.DTOs;

public record ActivityDetailDto(
    Guid Id,
    string Title,
    string Description,
    ActivityType ActivityType,
    ActivityStatus Status,
    Priority Priority,
    DateTime? DueDate,
    DateTime? CompletedAt,
    string? RelatedEntityType,
    Guid? RelatedEntityId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
