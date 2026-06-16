using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Customers.GetById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDetailDto>;
