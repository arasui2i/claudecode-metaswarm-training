using CRM.Application.Interfaces;
using MediatR;

namespace CRM.Application.Features.Dashboard.GetDashboardSummary;

public class GetDashboardSummaryHandler(IDashboardRepository repo) : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken ct) =>
        repo.GetSummaryAsync(ct);
}
