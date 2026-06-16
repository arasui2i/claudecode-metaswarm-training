using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Activities.GetById;

public record GetActivityByIdQuery(Guid Id) : IRequest<ActivityDetailDto>;
