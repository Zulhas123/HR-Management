using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class EmployeePromotionRepository : Repository<EmployeePromotion>
{
    private readonly HrSystemDbContext _dbContext;

    public EmployeePromotionRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<EmployeePromotion> Query() =>
        _dbContext.EmployeePromotions
            .Include(x => x.Employee)
            .Include(x => x.FromDesignation)
            .Include(x => x.ToDesignation);
}

