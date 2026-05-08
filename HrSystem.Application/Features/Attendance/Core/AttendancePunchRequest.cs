using HrSystem.Domain.Entities;

namespace HrSystem.Application.Attendance;

public enum AttendancePunchMode
{
    Auto = 0,
    CheckIn = 1,
    CheckOut = 2
}

public sealed class AttendancePunchRequest
{
    // Resolve employee by exactly one of these
    public int? EmployeeId { get; set; }
    public string? BiometricUserId { get; set; }
    public string? FaceProfileId { get; set; }
    public string? RfidCardId { get; set; }

    public AttendancePunchMode Mode { get; set; } = AttendancePunchMode.Auto;

    // If not provided, the server time is used
    public DateTimeOffset? PunchAtUtc { get; set; }

    // Optional shift hint; if not provided, existing record/employee settings may decide
    public int? ShiftId { get; set; }

    public AttendanceSource Source { get; set; } = AttendanceSource.Manual;
    public AttendanceDeviceVendor DeviceVendor { get; set; } = AttendanceDeviceVendor.Unknown;
    public string? DeviceId { get; set; }

    // Optional metadata
    public string? MobileDeviceId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public double? LocationAccuracyMeters { get; set; }

    public string? CapturedBy { get; set; }
    public string? Notes { get; set; }
}
