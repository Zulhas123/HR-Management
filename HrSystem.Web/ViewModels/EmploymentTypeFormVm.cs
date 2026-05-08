using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmploymentTypeFormVm
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = "";
}

