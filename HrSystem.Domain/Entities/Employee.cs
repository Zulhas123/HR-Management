using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class Employee : BaseEntity
{
    public required string EmployeeCode { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public DateOnly JoinDate { get; set; }

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int DesignationId { get; set; }
    public Designation? Designation { get; set; }

    public int EmploymentTypeId { get; set; }
    public EmploymentType? EmploymentType { get; set; }

    public string? NidNumber { get; set; }
    public string? TinNumber { get; set; }
}

