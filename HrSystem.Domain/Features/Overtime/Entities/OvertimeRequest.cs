using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum OvertimeRequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}

public enum OvertimeApprovalDecision
{
    Approved = 0,
    Rejected = 1
}

public sealed class OvertimeRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateOnly Date { get; set; }

    // Auto OT from attendance
    public int? AttendanceRecordId { get; set; }
    public AttendanceRecord? AttendanceRecord { get; set; }

    public bool IsHoliday { get; set; }
    public decimal PayMultiplier { get; set; } = 1.0m;

    public int CalculatedMinutes { get; set; }
    public int RequestedMinutes { get; set; }
    public int? ApprovedMinutes { get; set; }

    // Multi-level approval (MVP)
    public int ApprovalLevelsRequired { get; set; } = 1;
    public int ApprovalLevelsApproved { get; set; }

    public OvertimeRequestStatus Status { get; set; } = OvertimeRequestStatus.Pending;
    public DateTime? DecisionAtUtc { get; set; }
    public string? DecisionBy { get; set; }
    public string? DecisionNote { get; set; }

    public string? Reason { get; set; }

    public List<OvertimeApprovalStep> ApprovalSteps { get; set; } = [];
}

public sealed class OvertimeApprovalStep : BaseEntity
{
    public int OvertimeRequestId { get; set; }
    public OvertimeRequest? OvertimeRequest { get; set; }

    public int Level { get; set; }
    public OvertimeApprovalDecision Decision { get; set; } = OvertimeApprovalDecision.Approved;
    public DateTimeOffset DecidedAtUtc { get; set; }
    public string? DecidedBy { get; set; }
    public string? Note { get; set; }
}
