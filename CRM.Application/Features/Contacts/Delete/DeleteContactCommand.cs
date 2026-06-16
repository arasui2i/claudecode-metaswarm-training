using MediatR;

namespace CRM.Application.Features.Contacts.Delete;

public record DeleteContactCommand(Guid Id) : IRequest;
