using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Accounts.GetById;

public record GetAccountByIdQuery(Guid Id) : IRequest<AccountDetailDto>;
