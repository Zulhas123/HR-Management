using HrSystem.Application.Abstractions;
using HrSystem.Application.Attendance;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Services;

public sealed class AttendancePunchService(
    IAttendanceRecordRepository attendanceRecords,
    IEmployeeRepository employees,
    IRepository<Shift> shifts) : IAttendancePunchService
{
    public async Task<AttendanceRecord> PunchAsync(AttendancePunchRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await ResolveEmployeeAsync(request, cancellationToken);
        if (employee is null)
        {
            throw new InvalidOperationException("Employee not found for the provided identifiers.");
        }

        var punchAt = (request.PunchAtUtc ?? DateTimeOffset.UtcNow).ToLocalTime();
        var date = DateOnly.FromDateTime(punchAt.DateTime);
        var time = TimeOnly.FromDateTime(punchAt.DateTime);

        var record = await attendanceRecords.GetByEmployeeAndDateAsync(employee.Id, date, cancellationToken);
        record ??= new AttendanceRecord
        {
            EmployeeId = employee.Id,
            Date = date
        };

        if (request.ShiftId.HasValue)
        {
            record.ShiftId = request.ShiftId;
        }

        record.Source = request.Source;
        record.DeviceVendor = request.DeviceVendor;
        record.DeviceId = request.DeviceId;
        record.DeviceUserId = request.BiometricUserId ?? request.FaceProfileId ?? record.DeviceUserId;
        record.RfidCardId = request.RfidCardId ?? record.RfidCardId;
        record.MobileDeviceId = request.MobileDeviceId;
        record.Latitude = request.Latitude;
        record.Longitude = request.Longitude;
        record.LocationAccuracyMeters = request.LocationAccuracyMeters;
        record.CapturedAtUtc = (request.PunchAtUtc ?? DateTimeOffset.UtcNow).ToUniversalTime();
        record.CapturedBy = request.CapturedBy;
        record.Notes = request.Notes ?? record.Notes;

        ApplyPunchTime(record, time, request.Mode);
        AttendanceMetrics.ApplyMissingPunchStatus(record);

        if (record.ShiftId.HasValue)
        {
            var shift = await shifts.GetByIdAsync(record.ShiftId.Value, cancellationToken);
            if (shift is not null)
            {
                AttendanceMetrics.ApplyDerivedMetrics(record, shift);
            }
        }

        if (record.Id == 0)
        {
            await attendanceRecords.AddAsync(record, cancellationToken);
        }
        else
        {
            await attendanceRecords.UpdateAsync(record, cancellationToken);
        }

        await attendanceRecords.SaveChangesAsync(cancellationToken);
        return record;
    }

    private async Task<Employee?> ResolveEmployeeAsync(AttendancePunchRequest request, CancellationToken cancellationToken)
    {
        if (request.EmployeeId.HasValue)
        {
            return await employees.GetByIdAsync(request.EmployeeId.Value, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(request.BiometricUserId))
        {
            return await employees.GetByBiometricUserIdAsync(request.BiometricUserId, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(request.RfidCardId))
        {
            return await employees.GetByRfidCardIdAsync(request.RfidCardId, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(request.FaceProfileId))
        {
            return await employees.GetByFaceProfileIdAsync(request.FaceProfileId, cancellationToken);
        }

        return null;
    }

    private static void ApplyPunchTime(AttendanceRecord record, TimeOnly time, AttendancePunchMode mode)
    {
        if (mode == AttendancePunchMode.CheckIn)
        {
            record.CheckInTime = MinTime(record.CheckInTime, time) ?? time;
            return;
        }

        if (mode == AttendancePunchMode.CheckOut)
        {
            record.CheckOutTime = MaxTime(record.CheckOutTime, time) ?? time;
            return;
        }

        // Auto: prefer filling missing; otherwise keep earliest check-in + latest check-out.
        if (record.CheckInTime is null)
        {
            record.CheckInTime = time;
            return;
        }

        if (record.CheckOutTime is null)
        {
            record.CheckOutTime = time;
            return;
        }

        record.CheckInTime = MinTime(record.CheckInTime, time);
        record.CheckOutTime = MaxTime(record.CheckOutTime, time);
    }

    private static TimeOnly? MinTime(TimeOnly? a, TimeOnly b) => a is null ? b : (a.Value <= b ? a : b);
    private static TimeOnly? MaxTime(TimeOnly? a, TimeOnly b) => a is null ? b : (a.Value >= b ? a : b);
}
