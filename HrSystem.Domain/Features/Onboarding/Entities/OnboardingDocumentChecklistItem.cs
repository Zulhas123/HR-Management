using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class OnboardingDocumentChecklistItem : BaseEntity
{
    public int EmployeeOnboardingId { get; set; }
    public EmployeeOnboarding? EmployeeOnboarding { get; set; }

    public required string Name { get; set; }
    public bool IsRequired { get; set; } = true;

    public bool IsProvided { get; set; }
    public int? EmployeeDocumentId { get; set; }
    public EmployeeDocument? EmployeeDocument { get; set; }

    public string? Notes { get; set; }
}
