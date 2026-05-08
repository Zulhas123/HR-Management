using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class JobApplicationRepository : Repository<JobApplication>
{
    private readonly HrSystemDbContext _dbContext;

    public JobApplicationRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<JobApplication> Query() =>
        _dbContext.JobApplications
            .Include(x => x.JobPosting)
            .Include(x => x.Candidate);
}
