using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class OvertimePolicy : BaseEntity
{
    // Allows policy changes over time; pick the latest policy for MVP.
    public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    // Payroll multiplier for normal OT hours (typically 1.0x of OT rate; actual money calc is in payroll).
    public decimal NormalMultiplier { get; set; } = 1.0m;

    // Payroll multiplier for OT performed on holidays (double OT rules).
    public decimal HolidayMultiplier { get; set; } = 2.0m;

    // Default approval levels required for OT requests.
    public int ApprovalLevelsRequired { get; set; } = 1;
}
