using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum LeaveRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}

public sealed class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int LeaveTypeId { get; set; }
    public LeaveType? LeaveType { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public decimal TotalDays { get; set; }

    public string? Reason { get; set; }

    // Multi-level approval (MVP)
    public int ApprovalLevelsRequired { get; set; } = 1;
    public int ApprovalLevelsApproved { get; set; }

    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;
    public DateTime? DecisionAtUtc { get; set; }
    public string? DecisionBy { get; set; }
    public string? DecisionNote { get; set; }

    public List<LeaveApprovalStep> ApprovalSteps { get; set; } = [];
}
