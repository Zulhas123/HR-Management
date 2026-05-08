using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class JoiningFormVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeOnboardingId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime JoinDate { get; set; } = DateTime.Today;

    [Required]
    public int DepartmentId { get; set; }
    public List<SelectListItem> Departments { get; set; } = [];

    [Required]
    public int DesignationId { get; set; }
    public List<SelectListItem> Designations { get; set; } = [];

    [Required]
    public int EmploymentTypeId { get; set; }
    public List<SelectListItem> EmploymentTypes { get; set; } = [];

    [StringLength(2000)]
    public string? Notes { get; set; }
}

