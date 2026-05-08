using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum OffboardingStatus
{
    Initiated = 0,
    ClearanceInProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public sealed class EmployeeOffboarding : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateOnly LastWorkingDay { get; set; }
    public string? Reason { get; set; }

    public OffboardingStatus Status { get; set; } = OffboardingStatus.Initiated;
    public DateTimeOffset? CompletedAtUtc { get; set; }

    public ExitInterview? ExitInterview { get; set; }
    public List<OffboardingClearanceItem> ClearanceItems { get; set; } = [];
    public FinalSettlement? FinalSettlement { get; set; }
}
