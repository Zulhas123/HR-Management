using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class JobPostingFormVm
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = "";

    [StringLength(200)]
    public string? Department { get; set; }

    [StringLength(200)]
    public string? Location { get; set; }

    [StringLength(100)]
    public string? EmploymentType { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    public bool IsOpen { get; set; } = true;
}

