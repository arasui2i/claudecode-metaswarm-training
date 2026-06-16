using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public class TicketRepository(AppDbContext db) : ITicketRepository
{
    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Tickets
            .Include(t => t.Account)
            .Include(t => t.Contact)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<(IReadOnlyList<Ticket> Items, int Total)> GetPagedAsync(
        string search, int page, int pageSize, CancellationToken ct = default)
    {
        var query = db.Tickets.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.TicketNumber.Contains(search) || t.Subject.Contains(search));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<string> GetNextTicketNumberAsync(CancellationToken ct = default)
    {
        var count = await db.Tickets.IgnoreQueryFilters().CountAsync(ct);
        return $"TKT-{(count + 1):D5}";
    }

    public async Task AddAsync(Ticket ticket, CancellationToken ct = default)
    {
        db.Tickets.Add(ticket);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Ticket ticket, CancellationToken ct = default)
    {
        db.Tickets.Update(ticket);
        await db.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var ticket = await db.Tickets.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (ticket == null) return;
        ticket.IsDeleted = true;
        ticket.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
    }
}
