using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface IOvertimeRequestRepository : IRepository<OvertimeRequest>
{
    Task<OvertimeRequest?> GetByEmployeeAndDateAsync(
        int employeeId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OvertimeRequest>> ListByDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default);
}
