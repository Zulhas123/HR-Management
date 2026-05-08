using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class LeaveRequestFormVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    public List<SelectListItem> Employees { get; set; } = [];

    [Required]
    public int LeaveTypeId { get; set; }
    public List<SelectListItem> LeaveTypes { get; set; } = [];

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Reason { get; set; }
}

