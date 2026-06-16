using MediatR;

namespace CRM.Application.Features.Activities.Delete;

public record DeleteActivityCommand(Guid Id) : IRequest;
