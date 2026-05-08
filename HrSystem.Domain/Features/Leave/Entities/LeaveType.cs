using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class LeaveType : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal DefaultAnnualAllocation { get; set; }
    public bool IsPaid { get; set; } = true;

    // Leave policy (MVP)
    public int ApprovalLevelsRequired { get; set; } = 1;
    public bool CountWeekendsAsLeave { get; set; }
    public bool CountHolidaysAsLeave { get; set; }
    public bool AllowEncashment { get; set; }
    public decimal? MaxEncashmentDaysPerYear { get; set; }
}
