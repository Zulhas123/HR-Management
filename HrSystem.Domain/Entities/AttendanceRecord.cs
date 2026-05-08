using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum AttendanceSource
{
    Manual = 0,
    Biometric = 1,
    FaceRecognition = 2,
    Rfid = 3,
    GpsMobile = 4
}

public enum AttendanceDeviceVendor
{
    Unknown = 0,
    ZkTeco = 1,
    ESSL = 2,
    Hikvision = 3
}

public enum MissingPunchStatus
{
    None = 0,
    MissingCheckIn = 1,
    MissingCheckOut = 2,
    MissingBoth = 3
}

public sealed class AttendanceRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateOnly Date { get; set; }

    public int? ShiftId { get; set; }
    public Shift? Shift { get; set; }

    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }

    public AttendanceSource Source { get; set; } = AttendanceSource.Manual;
    public AttendanceDeviceVendor DeviceVendor { get; set; } = AttendanceDeviceVendor.Unknown;

    public string? DeviceId { get; set; }
    public string? DeviceUserId { get; set; }
    public string? RfidCardId { get; set; }

    public string? MobileDeviceId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public double? LocationAccuracyMeters { get; set; }

    public DateTimeOffset? CapturedAtUtc { get; set; }
    public string? CapturedBy { get; set; }

    public int? WorkedMinutes { get; set; }
    public int? LateMinutes { get; set; }
    public int? EarlyExitMinutes { get; set; }
    public MissingPunchStatus MissingPunchStatus { get; set; } = MissingPunchStatus.None;

    public DateTimeOffset? ProcessedAtUtc { get; set; }

    public string? Notes { get; set; }
}
