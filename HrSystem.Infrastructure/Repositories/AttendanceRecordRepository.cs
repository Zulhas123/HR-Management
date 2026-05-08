using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class AttendanceRecordRepository : Repository<AttendanceRecord>
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
}

