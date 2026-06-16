using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public class ActivityRepository(AppDbContext db) : IActivityRepository
{
    public Task<Activity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Activities.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Activity> Items, int Total)> GetPagedAsync(
        string search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Activities.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a => a.Title.Contains(search) || a.Description.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.DueDate).ThenBy(a => a.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(Activity activity, CancellationToken cancellationToken = default)
    {
        db.Activities.Add(activity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Activity activity, CancellationToken cancellationToken = default)
    {
        db.Activities.Update(activity);
        await db.SaveChangesAsync(cancellationToken);
    }
}
