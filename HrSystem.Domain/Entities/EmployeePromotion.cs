using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeePromotion : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public int? FromDesignationId { get; set; }
    public Designation? FromDesignation { get; set; }

    public int? ToDesignationId { get; set; }
    public Designation? ToDesignation { get; set; }

    public string? Note { get; set; }
}

