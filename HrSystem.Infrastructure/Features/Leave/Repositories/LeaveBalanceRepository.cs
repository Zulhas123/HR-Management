using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class LeaveBalanceRepository : Repository<LeaveBalance>, ILeaveBalanceRepository
{
    private readonly HrSystemDbContext _dbContext;

    public LeaveBalanceRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<LeaveBalance> Query() =>
        _dbContext.LeaveBalances
            .Include(x => x.Employee)
            .Include(x => x.LeaveType);

    public Task<LeaveBalance?> GetByEmployeeLeaveTypeYearAsync(int employeeId, int leaveTypeId, int year, CancellationToken cancellationToken = default) =>
        Query().FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId && x.Year == year, cancellationToken);

    public async Task<IReadOnlyList<LeaveBalance>> ListByEmployeeYearAsync(int employeeId, int year, CancellationToken cancellationToken = default) =>
        await Query()
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && x.Year == year)
            .OrderBy(x => x.LeaveType!.Name)
            .ToListAsync(cancellationToken);
}
