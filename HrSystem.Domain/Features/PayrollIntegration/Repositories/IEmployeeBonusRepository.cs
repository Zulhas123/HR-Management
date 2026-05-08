using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface IEmployeeBonusRepository : IRepository<EmployeeBonus>
{
    Task<IReadOnlyList<EmployeeBonus>> ListByAwardDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        bool onlyUnsynced = false,
        CancellationToken cancellationToken = default);
}
