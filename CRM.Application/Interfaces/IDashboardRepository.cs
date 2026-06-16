using CRM.Application.Features.Dashboard;

namespace CRM.Application.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default);
}
