using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Opportunities.Update;

public class UpdateOpportunityHandler(IOpportunityRepository repo) : IRequestHandler<UpdateOpportunityCommand>
{
    public async Task Handle(UpdateOpportunityCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Opportunity {request.Id} not found.");

        if (await repo.NameExistsAsync(request.Name, request.Id, cancellationToken))
            throw new InvalidOperationException("An opportunity with this name already exists.");

        opportunity.Name = request.Name;
        opportunity.Stage = request.Stage;
        opportunity.Amount = request.Amount;
        opportunity.Probability = request.Probability;
        opportunity.CloseDate = request.CloseDate;
        opportunity.Description = request.Description;
        opportunity.AccountId = request.AccountId;
        opportunity.ContactId = request.ContactId;
        opportunity.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(opportunity, cancellationToken);
    }
}
