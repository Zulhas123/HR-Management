using HrSystem.Application.Abstractions;
using HrSystem.Domain.Common;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Services;

public sealed class CrudService<T>(IRepository<T> repository) : ICrudService<T> where T : BaseEntity
{
    public Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default) =>
        repository.ListAsync(cancellationToken);

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await repository.UpdateAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        await repository.DeleteAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}

