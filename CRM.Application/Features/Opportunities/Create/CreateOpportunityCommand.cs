using CRM.Domain.Enums;
using MediatR;

namespace CRM.Application.Features.Opportunities.Create;

public class CreateOpportunityCommand : IRequest<Guid>
{
    public CreateOpportunityCommand(string name) => Name = name;

    public string Name { get; init; }
    public OpportunityStage Stage { get; init; } = OpportunityStage.Prospecting;
    public decimal Amount { get; init; }
    public int Probability { get; init; }
    public DateTime? CloseDate { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid? AccountId { get; init; }
    public Guid? ContactId { get; init; }
}
