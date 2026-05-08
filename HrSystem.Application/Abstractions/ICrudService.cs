using HrSystem.Domain.Common;

namespace HrSystem.Application.Abstractions;

public interface ICrudService<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

