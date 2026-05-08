using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class ExitInterview : BaseEntity
{
    public int EmployeeOffboardingId { get; set; }
    public EmployeeOffboarding? EmployeeOffboarding { get; set; }

    public DateOnly InterviewDate { get; set; }
    public string? Interviewer { get; set; }
    public string? Notes { get; set; }
}

