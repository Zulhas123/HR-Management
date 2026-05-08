using HrSystem.Application.Abstractions;
using HrSystem.Application.Attendance;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Services;

public sealed class AttendanceProcessingService(IAttendanceRecordRepository attendanceRecords) : IAttendanceProcessingService
{
    public async Task<IReadOnlyList<AttendanceRecord>> ProcessAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        bool recomputeProcessed = false,
        CancellationToken cancellationToken = default)
    {
        if (toInclusive < fromInclusive)
        {
            throw new ArgumentException("toInclusive must be >= fromInclusive");
        }

        var records = await attendanceRecords.ListByDateRangeAsync(fromInclusive, toInclusive, cancellationToken);
        var processedAtUtc = DateTimeOffset.UtcNow;

        foreach (var record in records)
        {
            if (!recomputeProcessed && record.ProcessedAtUtc is not null)
            {
                continue;
            }

            if (record.Shift is not null)
            {
                AttendanceMetrics.ApplyDerivedMetrics(record, record.Shift);
            }
            else
            {
                AttendanceMetrics.ApplyMissingPunchStatus(record);
            }

            record.ProcessedAtUtc = processedAtUtc;
        }

        await attendanceRecords.SaveChangesAsync(cancellationToken);
        return records;
    }
}

