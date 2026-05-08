using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class OvertimeRequestRepository : Repository<OvertimeRequest>, IOvertimeRequestRepository
{
    private readonly HrSystemDbContext _dbContext;

    public OvertimeRequestRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<OvertimeRequest> Query() =>
        _dbContext.OvertimeRequests
            .Include(x => x.Employee)
            .Include(x => x.AttendanceRecord)
            .Include(x => x.ApprovalSteps);

    public Task<OvertimeRequest?> GetByEmployeeAndDateAsync(
        int employeeId,
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Date == date, cancellationToken);

    public async Task<IReadOnlyList<OvertimeRequest>> ListByDateRangeAsync(
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
