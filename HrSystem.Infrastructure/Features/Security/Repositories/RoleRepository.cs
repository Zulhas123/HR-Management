using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Repositories;

public sealed class RoleRepository : Repository<AppRole>, IRoleRepository
{
    private readonly HrSystemDbContext _dbContext;

    public RoleRepository(HrSystemDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    protected override IQueryable<AppRole> Query() =>
        _dbContext.AppRoles
            .Include(x => x.RolePermissions)
            .ThenInclude(rp => rp.AppPermission);

    public Task<AppRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public async Task<IReadOnlyList<AppPermission>> ListPermissionsForRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var role = await Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);
        if (role is null)
        {
            return [];
        }

        return role.RolePermissions
            .Where(x => x.AppPermission is not null)
            .Select(x => x.AppPermission!)
            .OrderBy(x => x.Code)
            .ToList();
    }
}
