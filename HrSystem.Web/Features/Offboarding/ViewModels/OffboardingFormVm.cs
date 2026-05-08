using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class OffboardingFormVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }
    public List<SelectListItem> Employees { get; set; } = [];

    [Required]
    [DataType(DataType.Date)]
    public DateTime LastWorkingDay { get; set; } = DateTime.Today;

    [StringLength(2000)]
    public string? Reason { get; set; }

    public OffboardingStatus Status { get; set; } = OffboardingStatus.Initiated;
}
