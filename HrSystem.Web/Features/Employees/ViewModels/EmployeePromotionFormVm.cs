using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeePromotionFormVm
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime EffectiveDate { get; set; } = DateTime.Today;

    public int? FromDesignationId { get; set; }
    public int? ToDesignationId { get; set; }

    public List<SelectListItem> Designations { get; set; } = [];

    [StringLength(500)]
    public string? Note { get; set; }
}
