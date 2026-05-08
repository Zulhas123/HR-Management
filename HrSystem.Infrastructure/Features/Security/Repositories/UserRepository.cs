using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class UserRepository : Repository<AppUser>, IUserRepository
{
    private readonly HrSystemDbContext _dbContext;

    public UserRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<AppUser> Query() =>
        _dbContext.AppUsers
            .Include(x => x.UserRoles)
            .ThenInclude(ur => ur.AppRole!)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.AppPermission);

    public Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
}
