using MediatR;

namespace CRM.Application.Features.Customers.Delete;

public record DeleteCustomerCommand(Guid Id) : IRequest;
