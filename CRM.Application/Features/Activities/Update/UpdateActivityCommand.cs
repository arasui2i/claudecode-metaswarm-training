using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Activities.Update;

public class UpdateActivityCommand : IRequest
{
    public UpdateActivityCommand(Guid id, string title) { Id = id; Title = title; }
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; } = string.Empty;
    public ActivityType ActivityType { get; init; } = ActivityType.Task;
    public ActivityStatus Status { get; init; } = ActivityStatus.NotStarted;
    public Priority Priority { get; init; } = Priority.Medium;
    public DateTime? DueDate { get; init; }
    public string? RelatedEntityType { get; init; }
    public Guid? RelatedEntityId { get; init; }
}
