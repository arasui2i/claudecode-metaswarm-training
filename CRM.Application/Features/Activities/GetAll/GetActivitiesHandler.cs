using CRM.Application.Common;
using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Activities.GetAll;

public class GetActivitiesHandler(IActivityRepository repo) : IRequestHandler<GetActivitiesQuery, PagedResult<ActivitySummaryDto>>
{
    public async Task<PagedResult<ActivitySummaryDto>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repo.GetPagedAsync(request.Search, request.Page, request.PageSize, cancellationToken);
        var dtos = items.Select(a => new ActivitySummaryDto(
            a.Id, a.Title, a.ActivityType, a.Status, a.Priority,
            a.DueDate, a.RelatedEntityType, a.RelatedEntityId, a.CreatedAt)).ToList();
        return new PagedResult<ActivitySummaryDto>(dtos, total, request.Page, request.PageSize);
    }
}
