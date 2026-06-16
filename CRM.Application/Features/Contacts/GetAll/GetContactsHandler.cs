using CRM.Application.Common;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Contacts.GetAll;

public class GetContactsHandler(IContactRepository repo) : IRequestHandler<GetContactsQuery, PagedResult<ContactSummaryDto>>
{
    public async Task<PagedResult<ContactSummaryDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repo.GetPagedAsync(request.Search, request.Page, request.PageSize, cancellationToken);

        var dtos = items.Select(c => new ContactSummaryDto(
            c.Id, c.FirstName, c.LastName, c.Email, c.PhoneNumber,
            c.JobTitle, c.ContactType, c.CustomerId, c.CreatedAt
        )).ToList();

        return new PagedResult<ContactSummaryDto>(dtos, total, request.Page, request.PageSize);
    }
}
