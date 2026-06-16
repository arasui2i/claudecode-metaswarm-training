using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Customers.Delete;

public class DeleteCustomerHandler(ICustomerRepository repo) : IRequestHandler<DeleteCustomerCommand>
{
    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Customer {request.Id} not found.");

        customer.IsDeleted = true;
        customer.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(customer, cancellationToken);
    }
}
