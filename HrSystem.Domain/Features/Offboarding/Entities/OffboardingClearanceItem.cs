using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum ClearanceDecision
{
    Pending = 0,
    Cleared = 1,
    Blocked = 2
}

public sealed class OffboardingClearanceItem : BaseEntity
{
    public int EmployeeOffboardingId { get; set; }
    public EmployeeOffboarding? EmployeeOffboarding { get; set; }

    public required string DepartmentName { get; set; }
    public ClearanceDecision Decision { get; set; } = ClearanceDecision.Pending;

    public DateTimeOffset? DecidedAtUtc { get; set; }
    public string? DecidedBy { get; set; }
    public string? Note { get; set; }
}
