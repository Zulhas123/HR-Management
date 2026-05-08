using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeHandbookAcknowledgement : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int EmployeeHandbookId { get; set; }
    public EmployeeHandbook? EmployeeHandbook { get; set; }

    public DateTimeOffset AcknowledgedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

