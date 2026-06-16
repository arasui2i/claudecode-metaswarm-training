using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Activities.GetById;

public class GetActivityByIdHandler(IActivityRepository repo) : IRequestHandler<GetActivityByIdQuery, ActivityDetailDto>
{
    public async Task<ActivityDetailDto> Handle(GetActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Activity {request.Id} not found.");

        return new ActivityDetailDto(
            a.Id, a.Title, a.Description, a.ActivityType, a.Status, a.Priority,
            a.DueDate, a.CompletedAt, a.RelatedEntityType, a.RelatedEntityId,
            a.CreatedAt, a.UpdatedAt);
    }
}
