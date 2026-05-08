using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum OnboardingStatus
{
    Draft = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public sealed class EmployeeOnboarding : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public OnboardingStatus Status { get; set; } = OnboardingStatus.Draft;

    public EmployeeJoiningForm? JoiningForm { get; set; }
    public List<OnboardingDocumentChecklistItem> DocumentChecklist { get; set; } = [];
    public List<OnboardingOrientationItem> OrientationChecklist { get; set; } = [];
}
