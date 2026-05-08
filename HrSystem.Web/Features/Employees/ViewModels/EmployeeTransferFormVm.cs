using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeTransferFormVm
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EffectiveDate { get; set; } = DateTime.Today;

    public int? FromDepartmentId { get; set; }
    public int? ToDepartmentId { get; set; }

    public List<SelectListItem> Departments { get; set; } = [];

    [StringLength(500)]
    public string? Note { get; set; }
}
