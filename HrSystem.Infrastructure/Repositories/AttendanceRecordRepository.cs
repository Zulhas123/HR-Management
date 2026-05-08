using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class AttendanceRecordRepository : Repository<AttendanceRecord>, IAttendanceRecordRepository
{
    private readonly HrSystemDbContext _dbContext;

    public AttendanceRecordRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<AttendanceRecord> Query() =>
        _dbContext.AttendanceRecords
            .Include(x => x.Employee)
            .Include(x => x.Shift);

    public Task<AttendanceRecord?> GetByEmployeeAndDateAsync(int employeeId, DateOnly date, CancellationToken cancellationToken = default) =>
        Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Date == date, cancellationToken);

    public async Task<IReadOnlyList<AttendanceRecord>> ListByDateRangeAsync(DateOnly fromInclusive, DateOnly toInclusive, CancellationToken cancellationToken = default) =>
        await Query()
            .Where(x => x.Date >= fromInclusive && x.Date <= toInclusive)
            .OrderBy(x => x.Date)
            .ThenBy(x => x.EmployeeId)
            .ToListAsync(cancellationToken);
}
