using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class LeaveEncashmentRequestFormVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    public List<SelectListItem> Employees { get; set; } = [];

    [Required]
    public int LeaveTypeId { get; set; }
    public List<SelectListItem> LeaveTypes { get; set; } = [];

    [Range(2000, 2100)]
    public int Year { get; set; } = DateTime.Today.Year;

    [Range(0.01, 365)]
    public decimal DaysRequested { get; set; } = 1;
}
