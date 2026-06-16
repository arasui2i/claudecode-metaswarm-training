using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Opportunities.GetById;

public record GetOpportunityByIdQuery(Guid Id) : IRequest<OpportunityDetailDto>;
