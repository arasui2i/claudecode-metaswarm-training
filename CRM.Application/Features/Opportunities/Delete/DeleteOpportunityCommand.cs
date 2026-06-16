using MediatR;

namespace CRM.Application.Features.Opportunities.Delete;

public record DeleteOpportunityCommand(Guid Id) : IRequest;
