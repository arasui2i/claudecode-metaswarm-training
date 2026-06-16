using MediatR;

namespace CRM.Application.Features.Accounts.Delete;

public record DeleteAccountCommand(Guid Id) : IRequest;
