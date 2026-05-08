using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class EmployeeBonusRepository : Repository<EmployeeBonus>, IEmployeeBonusRepository
{
    private readonly HrSystemDbContext _dbContext;

    public EmployeeBonusRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<EmployeeBonus> Query() =>
        _dbContext.EmployeeBonuses
            .Include(x => x.Employee);

    public async Task<IReadOnlyList<EmployeeBonus>> ListByAwardDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        bool onlyUnsynced = false,
        CancellationToken cancellationToken = default)
    {
        var query = Query()
            .AsNoTracking()
            .Where(x => x.AwardDate >= fromInclusive && x.AwardDate <= toInclusive);

        if (onlyUnsynced)
        {
            query = query.Where(x => x.SyncedAtUtc == null);
        }

        return await query
            .OrderBy(x => x.AwardDate)
            .ThenBy(x => x.EmployeeId)
            .ToListAsync(cancellationToken);
    }
}
