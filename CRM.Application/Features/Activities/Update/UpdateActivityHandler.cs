using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Activities.Update;

public class UpdateActivityHandler(IActivityRepository repo) : IRequestHandler<UpdateActivityCommand>
{
    public async Task Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        var activity = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Activity {request.Id} not found.");

        activity.Title = request.Title;
        activity.Description = request.Description;
        activity.ActivityType = request.ActivityType;
        activity.Status = request.Status;
        activity.Priority = request.Priority;
        activity.DueDate = request.DueDate;
        activity.RelatedEntityType = request.RelatedEntityType;
        activity.RelatedEntityId = request.RelatedEntityId;
        activity.UpdatedAt = DateTime.UtcNow;

        if (request.Status == ActivityStatus.Completed && !activity.CompletedAt.HasValue)
            activity.CompletedAt = DateTime.UtcNow;

        await repo.UpdateAsync(activity, cancellationToken);
    }
}
