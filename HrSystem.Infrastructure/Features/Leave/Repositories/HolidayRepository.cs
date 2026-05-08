using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class HolidayRepository : Repository<Holiday>, IHolidayRepository
{
    private readonly HrSystemDbContext _dbContext;

    public HolidayRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Holiday>> ListByDateRangeAsync(DateOnly fromInclusive, DateOnly toInclusive, CancellationToken cancellationToken = default) =>
        await _dbContext.Holidays
            .AsNoTracking()
            .Where(h => h.Date >= fromInclusive && h.Date <= toInclusive)
            .OrderBy(h => h.Date)
            .ToListAsync(cancellationToken);
}
