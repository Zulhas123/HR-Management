using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeFamilyMember : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public required string Name { get; set; }
    public required string Relationship { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
}

