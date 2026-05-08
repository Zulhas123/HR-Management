using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class OnboardingDocumentChecklistItemVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeOnboardingId { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    public bool IsRequired { get; set; } = true;
    public bool IsProvided { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
