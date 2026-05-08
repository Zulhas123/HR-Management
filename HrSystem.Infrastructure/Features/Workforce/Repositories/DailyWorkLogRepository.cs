using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class DailyWorkLogRepository : Repository<DailyWorkLog>, IDailyWorkLogRepository
{
    private readonly HrSystemDbContext _dbContext;

    public DailyWorkLogRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<DailyWorkLog> Query() =>
        _dbContext.DailyWorkLogs
            .Include(x => x.Employee)
            .Include(x => x.EmployeeTask);

    public async Task<IReadOnlyList<DailyWorkLog>> ListByDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default) =>
        await Query()
            .AsNoTracking()
            .Where(x => x.Date >= fromInclusive && x.Date <= toInclusive)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
}
