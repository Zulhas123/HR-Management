using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class CandidateFormVm
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string FullName { get; set; } = "";

    [EmailAddress, StringLength(200)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? CvUrl { get; set; }

    [StringLength(2000)]
    public string? Notes { get; set; }
}

