using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Repositories;

public class ContactRepository(AppDbContext db) : IContactRepository
{
    public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Contacts.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Contact> Items, int Total)> GetPagedAsync(
        string search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Contacts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                c.Email.Contains(search) ||
                c.JobTitle.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId, CancellationToken cancellationToken = default) =>
        await db.Contacts.AnyAsync(
            c => c.Email == email && (excludeId == null || c.Id != excludeId),
            cancellationToken);

    public async Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        await db.Contacts.AddAsync(contact, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        db.Contacts.Update(contact);
        await db.SaveChangesAsync(cancellationToken);
    }
}
