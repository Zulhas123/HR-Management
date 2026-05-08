using System.ComponentModel.DataAnnotations;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeTaskFormVm
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Employee")]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = "";

    [MaxLength(2000)]
    public string? Description { get; set; }

    public EmployeeTaskPriority Priority { get; set; } = EmployeeTaskPriority.Medium;
    public EmployeeTaskStatus Status { get; set; } = EmployeeTaskStatus.Assigned;

    [Required]
    [Display(Name = "Assigned date")]
    public DateTime AssignedDate { get; set; } = DateTime.Today;

    [Display(Name = "Due date")]
    public DateTime? DueDate { get; set; }

    [Display(Name = "Completed date")]
    public DateTime? CompletedDate { get; set; }

    [Display(Name = "Estimated minutes")]
    public int? EstimatedMinutes { get; set; }

    [Display(Name = "Actual minutes")]
    public int? ActualMinutes { get; set; }

    [MaxLength(200)]
    [Display(Name = "Assigned by")]
    public string? AssignedBy { get; set; }

    public SelectList? Employees { get; set; }
}
