using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Lead> Items, int Total)> GetPagedAsync(string search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(Lead lead, CancellationToken cancellationToken = default);
    Task UpdateAsync(Lead lead, CancellationToken cancellationToken = default);
}
