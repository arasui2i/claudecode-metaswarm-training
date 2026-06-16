using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public class AccountRepository(AppDbContext db) : IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Accounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Account> Items, int Total)> GetPagedAsync(
        string search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Accounts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(a =>
                a.Name.Contains(search) ||
                a.Industry.Contains(search) ||
                a.Website.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(a => a.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken cancellationToken = default) =>
        await db.Accounts.AnyAsync(
            a => a.Name == name && (excludeId == null || a.Id != excludeId),
            cancellationToken);

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await db.Accounts.AddAsync(account, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        db.Accounts.Update(account);
        await db.SaveChangesAsync(cancellationToken);
    }
}
