using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public class OpportunityRepository(AppDbContext db) : IOpportunityRepository
{
    public async Task<Opportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Opportunities.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Opportunity> Items, int Total)> GetPagedAsync(
        string search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Opportunities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(o => o.Name.Contains(search) || o.Description.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken cancellationToken = default) =>
        await db.Opportunities.AnyAsync(
            o => o.Name == name && (excludeId == null || o.Id != excludeId),
            cancellationToken);

    public async Task AddAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        await db.Opportunities.AddAsync(opportunity, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        db.Opportunities.Update(opportunity);
        await db.SaveChangesAsync(cancellationToken);
    }
}
