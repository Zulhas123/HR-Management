using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class InterviewRepository : Repository<Interview>
{
    private readonly HrSystemDbContext _dbContext;

    public InterviewRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<Interview> Query() =>
        _dbContext.Interviews
            .Include(x => x.JobApplication)
            .ThenInclude(a => a!.JobPosting)
            .Include(x => x.JobApplication)
            .ThenInclude(a => a!.Candidate);
}
