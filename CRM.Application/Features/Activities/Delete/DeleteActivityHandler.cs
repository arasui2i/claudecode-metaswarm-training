using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Activities.Delete;

public class DeleteActivityHandler(IActivityRepository repo) : IRequestHandler<DeleteActivityCommand>
{
    public async Task Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
    {
        var activity = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Activity {request.Id} not found.");

        activity.IsDeleted = true;
        activity.UpdatedAt = DateTime.UtcNow;
        await repo.UpdateAsync(activity, cancellationToken);
    }
}
