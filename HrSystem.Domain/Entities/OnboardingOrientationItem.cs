using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class OnboardingOrientationItem : BaseEntity
{
    public int EmployeeOnboardingId { get; set; }
    public EmployeeOnboarding? EmployeeOnboarding { get; set; }

    public required string Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
    public string? CompletedBy { get; set; }
}

