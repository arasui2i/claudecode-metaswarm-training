using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Opportunities.GetById;

public class GetOpportunityByIdHandler(IOpportunityRepository repo) : IRequestHandler<GetOpportunityByIdQuery, OpportunityDetailDto>
{
    public async Task<OpportunityDetailDto> Handle(GetOpportunityByIdQuery request, CancellationToken cancellationToken)
    {
        var o = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Opportunity {request.Id} not found.");

        return new OpportunityDetailDto(
            o.Id, o.Name, o.Stage, o.Amount, o.Probability,
            o.CloseDate, o.Description, o.AccountId, o.ContactId,
            o.CreatedAt, o.UpdatedAt
        );
    }
}
