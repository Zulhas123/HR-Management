using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class EmployeeTransferRepository : Repository<EmployeeTransfer>
{
    private readonly HrSystemDbContext _dbContext;

    public EmployeeTransferRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<EmployeeTransfer> Query() =>
        _dbContext.EmployeeTransfers
            .Include(x => x.Employee)
            .Include(x => x.FromDepartment)
            .Include(x => x.ToDepartment);
}
