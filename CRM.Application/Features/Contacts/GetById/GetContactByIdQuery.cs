using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Contacts.GetById;

public record GetContactByIdQuery(Guid Id) : IRequest<ContactDetailDto>;
