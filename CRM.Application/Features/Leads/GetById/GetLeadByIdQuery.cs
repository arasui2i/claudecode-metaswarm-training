using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Leads.GetById;

public record GetLeadByIdQuery(Guid Id) : IRequest<LeadDetailDto>;
