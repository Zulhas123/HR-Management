using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeHandbookFormVm
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    public IFormFile? File { get; set; }
}

