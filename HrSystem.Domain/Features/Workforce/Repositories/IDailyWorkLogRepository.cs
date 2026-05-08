using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface IDailyWorkLogRepository : IRepository<DailyWorkLog>
{
    Task<IReadOnlyList<DailyWorkLog>> ListByDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default);
}
