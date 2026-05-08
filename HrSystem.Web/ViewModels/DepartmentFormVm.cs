using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class DepartmentFormVm
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }
}

