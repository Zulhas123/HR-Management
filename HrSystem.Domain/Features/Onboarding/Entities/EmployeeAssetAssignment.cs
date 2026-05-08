using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum AssetAssignmentStatus
{
    Assigned = 0,
    Returned = 1,
    Lost = 2,
    Damaged = 3
}

public sealed class EmployeeAssetAssignment : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public required string AssetName { get; set; }
    public string? AssetTag { get; set; }
    public string? SerialNumber { get; set; }

    public DateTimeOffset AssignedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? AssignedBy { get; set; }
    public string? ConditionOnAssign { get; set; }

    public AssetAssignmentStatus Status { get; set; } = AssetAssignmentStatus.Assigned;
    public DateTimeOffset? ReturnedAtUtc { get; set; }
    public string? ReturnedTo { get; set; }
    public string? ConditionOnReturn { get; set; }
}
