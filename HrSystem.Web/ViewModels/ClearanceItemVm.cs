using HrSystem.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class ClearanceItemVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeOffboardingId { get; set; }

    [Required, StringLength(200)]
    public string DepartmentName { get; set; } = "";

    public ClearanceDecision Decision { get; set; } = ClearanceDecision.Pending;

    [StringLength(1000)]
    public string? Note { get; set; }
}

