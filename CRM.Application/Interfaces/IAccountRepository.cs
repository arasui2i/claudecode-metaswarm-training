using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Account> Items, int Total)> GetPagedAsync(string search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Account account, CancellationToken cancellationToken = default);
    Task UpdateAsync(Account account, CancellationToken cancellationToken = default);
}
