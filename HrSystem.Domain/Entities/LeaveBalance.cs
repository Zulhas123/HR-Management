using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class LeaveBalance : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int LeaveTypeId { get; set; }
    public LeaveType? LeaveType { get; set; }

    public int Year { get; set; }

    public decimal AllocatedDays { get; set; }
    public decimal UsedDays { get; set; }
    public decimal EncashmentDays { get; set; }

    public DateTimeOffset? LastRecalculatedAtUtc { get; set; }
}

