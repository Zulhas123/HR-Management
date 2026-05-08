using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using HrSystem.Domain.Entities;

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

    public AttendanceSource Source { get; set; } = AttendanceSource.Manual;
    public AttendanceDeviceVendor DeviceVendor { get; set; } = AttendanceDeviceVendor.Unknown;

    [StringLength(100)]
    public string? DeviceId { get; set; }

    [StringLength(100)]
    public string? DeviceUserId { get; set; }

    [StringLength(100)]
    public string? RfidCardId { get; set; }

    [StringLength(200)]
    public string? MobileDeviceId { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public double? LocationAccuracyMeters { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
