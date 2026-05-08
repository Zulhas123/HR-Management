using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class EmployeeTaskRepository : Repository<EmployeeTask>, IEmployeeTaskRepository
{
    private readonly HrSystemDbContext _dbContext;

    public EmployeeTaskRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<EmployeeTask> Query() =>
        _dbContext.EmployeeTasks
            .Include(x => x.Employee);

    public async Task<IReadOnlyList<EmployeeTask>> ListByAssignedDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default) =>
        await Query()
            .AsNoTracking()
            .Where(x => x.AssignedDate >= fromInclusive && x.AssignedDate <= toInclusive)
            .OrderByDescending(x => x.AssignedDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
}
