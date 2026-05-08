using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface IRoleRepository : IRepository<AppRole>
{
    Task<AppRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppPermission>> ListPermissionsForRoleAsync(int roleId, CancellationToken cancellationToken = default);
}
