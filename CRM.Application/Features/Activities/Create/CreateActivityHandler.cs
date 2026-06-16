using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using MediatR;

namespace CRM.Application.Features.Activities.Create;

public class CreateActivityHandler(IActivityRepository repo) : IRequestHandler<CreateActivityCommand, Guid>
{
    public async Task<Guid> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
    {
        var activity = new Activity
        {
            Title = request.Title,
            Description = request.Description,
            ActivityType = request.ActivityType,
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            RelatedEntityType = request.RelatedEntityType,
            RelatedEntityId = request.RelatedEntityId,
        };
        await repo.AddAsync(activity, cancellationToken);
        return activity.Id;
    }
}
