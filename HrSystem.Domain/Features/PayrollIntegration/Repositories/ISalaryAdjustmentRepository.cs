using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface ISalaryAdjustmentRepository : IRepository<SalaryAdjustment>
{
    Task<IReadOnlyList<SalaryAdjustment>> ListByEffectiveDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        bool onlyUnsynced = false,
        CancellationToken cancellationToken = default);
}
