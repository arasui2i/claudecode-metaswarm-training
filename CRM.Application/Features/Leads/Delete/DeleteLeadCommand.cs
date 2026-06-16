using MediatR;

namespace CRM.Application.Features.Leads.Delete;

public record DeleteLeadCommand(Guid Id) : IRequest;
