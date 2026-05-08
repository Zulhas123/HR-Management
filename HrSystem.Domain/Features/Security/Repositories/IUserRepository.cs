using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface IUserRepository : IRepository<AppUser>
{
    Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
