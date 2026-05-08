using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum LeaveApprovalDecision
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public sealed class LeaveApprovalStep : BaseEntity
{
    public int LeaveRequestId { get; set; }
    public LeaveRequest? LeaveRequest { get; set; }

    public int Level { get; set; }
    public LeaveApprovalDecision Decision { get; set; } = LeaveApprovalDecision.Pending;

    public DateTimeOffset? DecidedAtUtc { get; set; }
    public string? DecidedBy { get; set; }
    public string? Note { get; set; }
}
