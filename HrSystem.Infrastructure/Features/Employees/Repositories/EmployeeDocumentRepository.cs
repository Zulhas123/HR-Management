using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class EmployeeDocumentRepository : Repository<EmployeeDocument>
{
    private readonly HrSystemDbContext _dbContext;

    public EmployeeDocumentRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<EmployeeDocument> Query() =>
        _dbContext.EmployeeDocuments.Include(d => d.Employee);
}
