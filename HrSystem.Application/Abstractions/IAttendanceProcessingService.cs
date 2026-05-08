using HrSystem.Domain.Entities;

namespace HrSystem.Application.Abstractions;

public interface IAttendanceProcessingService
{
    Task<IReadOnlyList<AttendanceRecord>> ProcessAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        bool recomputeProcessed = false,
        CancellationToken cancellationToken = default);
}

