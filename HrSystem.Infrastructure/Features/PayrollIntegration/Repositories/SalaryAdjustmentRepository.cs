using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class SalaryAdjustmentRepository : Repository<SalaryAdjustment>, ISalaryAdjustmentRepository
{
    private readonly HrSystemDbContext _dbContext;

    public SalaryAdjustmentRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<SalaryAdjustment> Query() =>
        _dbContext.SalaryAdjustments
            .Include(x => x.Employee);

    public async Task<IReadOnlyList<SalaryAdjustment>> ListByEffectiveDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        bool onlyUnsynced = false,
        CancellationToken cancellationToken = default)
    {
        var query = Query()
            .AsNoTracking()
            .Where(x => x.EffectiveDate >= fromInclusive && x.EffectiveDate <= toInclusive);

        if (onlyUnsynced)
        {
            query = query.Where(x => x.SyncedAtUtc == null);
        }

        return await query
            .OrderBy(x => x.EffectiveDate)
            .ThenBy(x => x.EmployeeId)
            .ToListAsync(cancellationToken);
    }
}
