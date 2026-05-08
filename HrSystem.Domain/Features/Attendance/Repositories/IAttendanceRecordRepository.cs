using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface IAttendanceRecordRepository : IRepository<AttendanceRecord>
{
    Task<AttendanceRecord?> GetByEmployeeAndDateAsync(
        int employeeId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AttendanceRecord>> ListByDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default);
}
