using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class EmployeeDocumentFormVm
{
    public int EmployeeId { get; set; }

    [Required, StringLength(200)]
    public string DocumentName { get; set; } = "";

    [StringLength(100)]
    public string? DocumentType { get; set; }

    [Required]
    public IFormFile? File { get; set; }
}

