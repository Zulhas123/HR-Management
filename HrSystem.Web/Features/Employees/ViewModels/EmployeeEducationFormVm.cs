using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeEducationFormVm
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required, StringLength(200)]
    public string Degree { get; set; } = "";

    [StringLength(300)]
    public string? Institution { get; set; }

    [Range(1900, 2100)]
    public int? PassingYear { get; set; }

    [StringLength(100)]
    public string? Result { get; set; }
}
