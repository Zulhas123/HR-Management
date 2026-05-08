using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class AttendanceRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateOnly Date { get; set; }

    public int? ShiftId { get; set; }
    public Shift? Shift { get; set; }

    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }

    public string? Notes { get; set; }
}

