using HrSystem.Domain.Common;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HrSystem.Infrastructure.Repositories;

public class Repository<T>(HrSystemDbContext dbContext) : IRepository<T> where T : BaseEntity
{
    protected virtual IQueryable<T> Query() => dbContext.Set<T>();

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default) =>
        await Query().AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(cancellationToken);

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null
            ? Query().AsNoTracking().CountAsync(cancellationToken)
            : Query().AsNoTracking().CountAsync(predicate, cancellationToken);

    public Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        dbContext.AddAsync(entity, cancellationToken).AsTask();

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        dbContext.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        dbContext.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
