using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class EmployeeRepository : Repository<Employee>
{
    private readonly HrSystemDbContext _dbContext;

    public EmployeeRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<Employee> Query() =>
        _dbContext.Employees
            .Include(x => x.Department)
            .Include(x => x.Designation)
            .Include(x => x.EmploymentType);
}
