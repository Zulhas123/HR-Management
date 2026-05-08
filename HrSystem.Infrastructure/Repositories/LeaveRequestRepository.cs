using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class LeaveRequestRepository : Repository<LeaveRequest>
{
    private readonly HrSystemDbContext _dbContext;

    public LeaveRequestRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<LeaveRequest> Query() =>
        _dbContext.LeaveRequests
            .Include(x => x.Employee)
            .Include(x => x.LeaveType);
}

