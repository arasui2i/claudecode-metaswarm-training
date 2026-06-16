using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Customers.Update;

public class UpdateCustomerHandler(ICustomerRepository repo) : IRequestHandler<UpdateCustomerCommand>
{
    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Customer {request.Id} not found.");

        if (await repo.EmailExistsAsync(request.Email, request.Id, cancellationToken))
            throw new InvalidOperationException("A customer with this email already exists.");

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.Company = request.Company;
        customer.PhoneNumber = request.PhoneNumber;
        customer.Status = request.Status;
        customer.JobTitle = request.JobTitle;
        customer.Gender = request.Gender;
        customer.Age = request.Age;
        customer.Industry = request.Industry;
        customer.AnnualIncome = request.AnnualIncome;
        customer.EmployeeCount = request.EmployeeCount;
        customer.HeadquartersAddress = request.HeadquartersAddress;
        customer.UpdatedAt = DateTime.UtcNow;

        await repo.UpdateAsync(customer, cancellationToken);
    }
}
