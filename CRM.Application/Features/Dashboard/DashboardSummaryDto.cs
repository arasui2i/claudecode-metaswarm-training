namespace CRM.Application.Features.Dashboard;

public record TicketsByStatusDto(int New, int InProgress, int Pending, int Resolved, int Closed);

public record DashboardSummaryDto(
    int CurrentMonthLeads,
    int ConvertedCustomersThisMonth,
    TicketsByStatusDto TicketsByStatus);
