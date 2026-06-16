using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using MediatR;

namespace CRM.Application.Features.Opportunities.Create;

public class CreateOpportunityHandler(IOpportunityRepository repo) : IRequestHandler<CreateOpportunityCommand, Guid>
{
    public async Task<Guid> Handle(CreateOpportunityCommand request, CancellationToken cancellationToken)
    {
        if (await repo.NameExistsAsync(request.Name, null, cancellationToken))
            throw new InvalidOperationException("An opportunity with this name already exists.");

        var opportunity = new Opportunity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Stage = request.Stage,
            Amount = request.Amount,
            Probability = request.Probability,
            CloseDate = request.CloseDate,
            Description = request.Description,
            AccountId = request.AccountId,
            ContactId = request.ContactId,
        };

        await repo.AddAsync(opportunity, cancellationToken);
        return opportunity.Id;
    }
}
