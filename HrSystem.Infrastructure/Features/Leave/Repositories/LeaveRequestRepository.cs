using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
{
    private readonly HrSystemDbContext _dbContext;

    public LeaveRequestRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<LeaveRequest> Query() =>
        _dbContext.LeaveRequests
            .Include(x => x.Employee)
            .Include(x => x.LeaveType)
            .Include(x => x.ApprovalSteps);

    public async Task<IReadOnlyList<LeaveRequest>> ListApprovedOverlappingDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default) =>
        await Query()
            .AsNoTracking()
            .Where(x =>
                x.Status == LeaveRequestStatus.Approved &&
                x.StartDate <= toInclusive &&
                x.EndDate >= fromInclusive)
            .OrderBy(x => x.StartDate)
            .ThenBy(x => x.EmployeeId)
            .ToListAsync(cancellationToken);
}
