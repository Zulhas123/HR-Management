using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class OvertimeRequestFormVm
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Employee")]
    public int EmployeeId { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.Today;

    [Range(0, 24 * 60)]
    [Display(Name = "Requested minutes")]
    public int RequestedMinutes { get; set; }

    [Range(0, 24 * 60)]
    [Display(Name = "Calculated minutes")]
    public int CalculatedMinutes { get; set; }

    public int? AttendanceRecordId { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }
}
