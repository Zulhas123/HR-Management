using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum LeaveEncashmentStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Paid = 3
}

public sealed class LeaveEncashmentRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int LeaveTypeId { get; set; }
    public LeaveType? LeaveType { get; set; }

    public int Year { get; set; }
    public decimal DaysRequested { get; set; }

    public LeaveEncashmentStatus Status { get; set; } = LeaveEncashmentStatus.Pending;
    public DateTimeOffset RequestedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? DecisionAtUtc { get; set; }
    public string? DecisionBy { get; set; }
    public string? DecisionNote { get; set; }
}
