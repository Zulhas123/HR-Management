using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeExperienceFormVm
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required, StringLength(200)]
    public string CompanyName { get; set; } = "";

    [StringLength(200)]
    public string? Designation { get; set; }

    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

