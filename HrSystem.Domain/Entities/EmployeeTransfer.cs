using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeTransfer : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public int? FromDepartmentId { get; set; }
    public Department? FromDepartment { get; set; }

    public int? ToDepartmentId { get; set; }
    public Department? ToDepartment { get; set; }

    public string? Note { get; set; }
}

