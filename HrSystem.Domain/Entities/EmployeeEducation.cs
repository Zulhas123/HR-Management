using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeEducation : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public required string Degree { get; set; }
    public string? Institution { get; set; }
    public int? PassingYear { get; set; }
    public string? Result { get; set; }
}

