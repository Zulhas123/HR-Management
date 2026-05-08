using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class OnboardingOrientationItemVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeOnboardingId { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    public bool IsCompleted { get; set; }

    [StringLength(200)]
    public string? CompletedBy { get; set; }
}

