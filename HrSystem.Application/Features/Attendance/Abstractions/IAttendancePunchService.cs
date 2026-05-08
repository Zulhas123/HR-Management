using HrSystem.Application.Attendance;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Abstractions;

public interface IAttendancePunchService
{
    Task<AttendanceRecord> PunchAsync(AttendancePunchRequest request, CancellationToken cancellationToken = default);
}
