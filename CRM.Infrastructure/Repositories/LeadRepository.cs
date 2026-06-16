using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public class LeadRepository(AppDbContext db) : ILeadRepository
{
    public async Task<Lead?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Leads.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Lead> Items, int Total)> GetPagedAsync(
        string search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Leads.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(l =>
                l.FirstName.Contains(search) ||
                l.LastName.Contains(search) ||
                l.Email.Contains(search) ||
                l.Company.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId, CancellationToken cancellationToken = default) =>
        await db.Leads.AnyAsync(
            l => l.Email == email && (excludeId == null || l.Id != excludeId),
            cancellationToken);

    public async Task AddAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        await db.Leads.AddAsync(lead, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        db.Leads.Update(lead);
        await db.SaveChangesAsync(cancellationToken);
    }
}
