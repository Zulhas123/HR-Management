using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class LeaveEncashmentRequestRepository : Repository<LeaveEncashmentRequest>
{
    private readonly HrSystemDbContext _dbContext;

    public LeaveEncashmentRequestRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<LeaveEncashmentRequest> Query() =>
        _dbContext.LeaveEncashmentRequests
            .Include(x => x.Employee)
            .Include(x => x.LeaveType);
}
