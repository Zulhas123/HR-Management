using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeBonus : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    // Date the bonus should be considered for payroll.
    public DateOnly AwardDate { get; set; }

    public decimal Amount { get; set; }

    public string? Title { get; set; }
    public string? Notes { get; set; }

    // For payroll integration exports/sync.
    public DateTimeOffset? SyncedAtUtc { get; set; }
}
