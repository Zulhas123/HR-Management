using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class AttendanceRecordFormVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    public List<SelectListItem> Employees { get; set; } = [];

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    public int? ShiftId { get; set; }
    public List<SelectListItem> Shifts { get; set; } = [];

    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

