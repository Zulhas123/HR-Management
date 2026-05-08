using System.ComponentModel.DataAnnotations;

namespace HrSystem.Web.ViewModels;

public sealed class ExitInterviewVm
{
    public int Id { get; set; }

    [Required]
    public int EmployeeOffboardingId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime InterviewDate { get; set; } = DateTime.Today;

    [StringLength(200)]
    public string? Interviewer { get; set; }

    [StringLength(4000)]
    public string? Notes { get; set; }
}

