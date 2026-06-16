using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Activity> Items, int Total)> GetPagedAsync(string search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Activity activity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Activity activity, CancellationToken cancellationToken = default);
}
