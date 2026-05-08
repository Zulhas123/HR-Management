using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeEmergencyContact : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public required string Name { get; set; }
    public string? Relationship { get; set; }
    public required string Phone { get; set; }
    public string? Address { get; set; }
    public bool IsPrimary { get; set; } = true;
}
