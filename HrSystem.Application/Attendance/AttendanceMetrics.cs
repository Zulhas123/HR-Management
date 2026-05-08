using HrSystem.Domain.Entities;

namespace HrSystem.Application.Attendance;

public static class AttendanceMetrics
{
    public static void ApplyDerivedMetrics(AttendanceRecord record, Shift shift)
    {
        ApplyMissingPunchStatus(record);

        if (record.CheckInTime is null || record.CheckOutTime is null)
        {
            record.WorkedMinutes = null;
            record.LateMinutes = null;
            record.EarlyExitMinutes = null;
            return;
        }

        var workedMinutes = ComputeWorkedMinutes(record.CheckInTime.Value, record.CheckOutTime.Value, shift.IsOvernight);
        record.WorkedMinutes = workedMinutes;

        record.LateMinutes = ComputeLateMinutes(record.CheckInTime.Value, shift);
        record.EarlyExitMinutes = ComputeEarlyExitMinutes(record.CheckOutTime.Value, shift, workedMinutes);
    }

    public static void ApplyMissingPunchStatus(AttendanceRecord record)
    {
        if (record.CheckInTime is null && record.CheckOutTime is null)
        {
            record.MissingPunchStatus = MissingPunchStatus.MissingBoth;
            return;
        }

        if (record.CheckInTime is null)
        {
            record.MissingPunchStatus = MissingPunchStatus.MissingCheckIn;
            return;
        }

        if (record.CheckOutTime is null)
        {
            record.MissingPunchStatus = MissingPunchStatus.MissingCheckOut;
            return;
        }

        record.MissingPunchStatus = MissingPunchStatus.None;
    }

    public static int ComputeWorkedMinutes(TimeOnly checkIn, TimeOnly checkOut, bool isOvernight)
    {
        var start = checkIn.ToTimeSpan();
        var end = checkOut.ToTimeSpan();

        if (!isOvernight)
        {
            return (int)Math.Max(0, (end - start).TotalMinutes);
        }

        // Overnight: allow crossing midnight.
        if (end >= start)
        {
            return (int)Math.Max(0, (end - start).TotalMinutes);
        }

        var untilMidnight = TimeSpan.FromHours(24) - start;
        return (int)Math.Max(0, (untilMidnight + end).TotalMinutes);
    }

    public static int ComputeLateMinutes(TimeOnly checkIn, Shift shift)
    {
        if (shift.IsFlexibleHours)
        {
            var latest = shift.FlexInEndTime ?? shift.StartTime;
            if (checkIn <= latest)
            {
                return 0;
            }

            return (int)Math.Ceiling((checkIn.ToTimeSpan() - latest.ToTimeSpan()).TotalMinutes);
        }

        var grace = TimeSpan.FromMinutes(Math.Max(0, shift.GraceMinutes));
        var allowed = shift.StartTime.ToTimeSpan() + grace;
        var actual = checkIn.ToTimeSpan();
        if (actual <= allowed)
        {
            return 0;
        }

        return (int)Math.Ceiling((actual - allowed).TotalMinutes);
    }

    public static int ComputeEarlyExitMinutes(TimeOnly checkOut, Shift shift, int workedMinutes)
    {
        var requiredMinutes = shift.RequiredWorkMinutes;
        if (requiredMinutes.HasValue && requiredMinutes.Value > 0)
        {
            return Math.Max(0, requiredMinutes.Value - workedMinutes);
        }

        // Fallback: compare to shift end time (for non-flex shifts).
        if (shift.IsFlexibleHours)
        {
            return 0;
        }

        var expected = shift.EndTime.ToTimeSpan();
        var actual = checkOut.ToTimeSpan();
        if (actual >= expected)
        {
            return 0;
        }

        return (int)Math.Ceiling((expected - actual).TotalMinutes);
    }
}
