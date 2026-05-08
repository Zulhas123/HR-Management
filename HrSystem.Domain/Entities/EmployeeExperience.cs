using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeExperience : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public required string CompanyName { get; set; }
    public string? Designation { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Notes { get; set; }
}

