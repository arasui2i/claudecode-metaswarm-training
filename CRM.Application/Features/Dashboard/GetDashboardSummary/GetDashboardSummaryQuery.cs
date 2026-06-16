using MediatR;

namespace CRM.Application.Features.Dashboard.GetDashboardSummary;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;
