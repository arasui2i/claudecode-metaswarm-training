using CRM.Application.Features.Dashboard;
using CRM.Application.Interfaces;
using CRM.Domain.Enums;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public class DashboardRepository(AppDbContext db) : IDashboardRepository
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var monthEnd = monthStart.AddMonths(1);

        var currentMonthLeads = await db.Leads
            .Where(l => l.CreatedAt >= monthStart && l.CreatedAt < monthEnd)
            .CountAsync(ct);

        var convertedThisMonth = await db.Leads
            .Where(l => l.Status == LeadStatus.Converted &&
                        l.ConvertedAt >= monthStart && l.ConvertedAt < monthEnd)
            .CountAsync(ct);

        var ticketCounts = await db.Tickets
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        int Count(TicketStatus s) => ticketCounts.FirstOrDefault(x => x.Status == s)?.Count ?? 0;

        return new DashboardSummaryDto(
            currentMonthLeads,
            convertedThisMonth,
            new TicketsByStatusDto(
                Count(TicketStatus.Open),
                Count(TicketStatus.InProgress),
                Count(TicketStatus.Pending),
                Count(TicketStatus.Resolved),
                Count(TicketStatus.Closed)));
    }
}
