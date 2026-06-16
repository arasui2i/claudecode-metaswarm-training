using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Opportunities.Delete;

public class DeleteOpportunityHandler(IOpportunityRepository repo) : IRequestHandler<DeleteOpportunityCommand>
{
    public async Task Handle(DeleteOpportunityCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Opportunity {request.Id} not found.");

        opportunity.IsDeleted = true;
        opportunity.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(opportunity, cancellationToken);
    }
}
