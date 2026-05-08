using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.ViewModels;

public sealed class DailyWorkLogFormVm
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Employee")]
    public int EmployeeId { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.Today;

    [Range(0, 24 * 60)]
    [Display(Name = "Minutes worked")]
    public int MinutesWorked { get; set; }

    [Display(Name = "Work from home")]
    public bool IsWorkFromHome { get; set; }

    [Display(Name = "Task (optional)")]
    public int? EmployeeTaskId { get; set; }

    [MaxLength(2000)]
    public string? Summary { get; set; }

    public SelectList? Employees { get; set; }
    public SelectList? Tasks { get; set; }
}
