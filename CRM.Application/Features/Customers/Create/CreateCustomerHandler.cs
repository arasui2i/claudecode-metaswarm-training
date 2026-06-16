using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using MediatR;

namespace CRM.Application.Features.Customers.Create;

public class CreateCustomerHandler(ICustomerRepository repo) : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await repo.EmailExistsAsync(request.Email, null, cancellationToken))
            throw new InvalidOperationException("A customer with this email already exists.");

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Company = request.Company,
            PhoneNumber = request.PhoneNumber,
            Status = request.Status,
            JobTitle = request.JobTitle,
            Gender = request.Gender,
            Age = request.Age,
            Industry = request.Industry,
            AnnualIncome = request.AnnualIncome,
            EmployeeCount = request.EmployeeCount,
            HeadquartersAddress = request.HeadquartersAddress,
        };

        await repo.AddAsync(customer, cancellationToken);
        return customer.Id;
    }
}
