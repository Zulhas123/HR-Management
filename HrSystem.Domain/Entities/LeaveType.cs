using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class LeaveType : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal DefaultAnnualAllocation { get; set; }
    public bool IsPaid { get; set; } = true;
}

