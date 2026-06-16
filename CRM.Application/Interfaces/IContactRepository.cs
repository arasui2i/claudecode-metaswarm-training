using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Contact> Items, int Total)> GetPagedAsync(string search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Contact contact, CancellationToken cancellationToken = default);
    Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default);
}
