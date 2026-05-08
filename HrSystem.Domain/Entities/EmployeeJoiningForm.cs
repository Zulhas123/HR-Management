using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeJoiningForm : BaseEntity
{
    public int EmployeeOnboardingId { get; set; }
    public EmployeeOnboarding? EmployeeOnboarding { get; set; }

    public DateOnly JoinDate { get; set; }

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int DesignationId { get; set; }
    public Designation? Designation { get; set; }

    public int EmploymentTypeId { get; set; }
    public EmploymentType? EmploymentType { get; set; }

    public string? Notes { get; set; }
}

