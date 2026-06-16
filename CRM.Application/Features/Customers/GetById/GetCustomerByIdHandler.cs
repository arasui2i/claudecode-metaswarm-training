using CRM.Application.DTOs;
using CRM.Application.Exceptions;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Customers.GetById;

public class GetCustomerByIdHandler(ICustomerRepository repo) : IRequestHandler<GetCustomerByIdQuery, CustomerDetailDto>
{
    public async Task<CustomerDetailDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var c = await repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Customer {request.Id} not found.");

        return new CustomerDetailDto(
            c.Id, c.FirstName, c.LastName, c.Company, c.Status, c.JobTitle,
            c.Gender, c.Age, c.Email, c.PhoneNumber, c.Industry,
            c.AnnualIncome, c.EmployeeCount, c.HeadquartersAddress,
            c.CreatedAt, c.UpdatedAt
        );
    }
}
