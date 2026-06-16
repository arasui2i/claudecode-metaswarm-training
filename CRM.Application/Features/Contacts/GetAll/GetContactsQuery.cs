using CRM.Application.Common;
using CRM.Application.DTOs;
using MediatR;

namespace CRM.Application.Features.Contacts.GetAll;

public record GetContactsQuery : IRequest<PagedResult<ContactSummaryDto>>
{
    public string Search { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
