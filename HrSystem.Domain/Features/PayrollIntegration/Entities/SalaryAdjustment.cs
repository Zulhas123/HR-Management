using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum SalaryAdjustmentKind
{
    Increment = 0,
    Deduction = 1,
    SetSalary = 2
}

public sealed class SalaryAdjustment : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    // Effective date the payroll system should apply this adjustment from.
    public DateOnly EffectiveDate { get; set; }

    public SalaryAdjustmentKind Kind { get; set; } = SalaryAdjustmentKind.Increment;

    // For Increment/Deduction this is a delta amount. For SetSalary this is the new salary amount.
    public decimal Amount { get; set; }

    public string? Reason { get; set; }

    // For payroll integration exports/sync.
    public DateTimeOffset? SyncedAtUtc { get; set; }
}
