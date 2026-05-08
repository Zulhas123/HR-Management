using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeHandbookAcknowledgeVm
{
    [Required]
    public int EmployeeHandbookId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    public List<SelectListItem> Employees { get; set; } = [];
}

