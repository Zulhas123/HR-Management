using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class LeaveTypeFormVm
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, 365)]
    public decimal DefaultAnnualAllocation { get; set; } = 10;

    public bool IsPaid { get; set; } = true;
}

