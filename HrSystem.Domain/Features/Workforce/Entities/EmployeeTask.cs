using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public enum EmployeeTaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum EmployeeTaskStatus
{
    Assigned = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public sealed class EmployeeTask : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public required string Title { get; set; }
    public string? Description { get; set; }

    public EmployeeTaskPriority Priority { get; set; } = EmployeeTaskPriority.Medium;
    public EmployeeTaskStatus Status { get; set; } = EmployeeTaskStatus.Assigned;

    public DateOnly AssignedDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public DateOnly? CompletedDate { get; set; }

    public int? EstimatedMinutes { get; set; }
    public int? ActualMinutes { get; set; }

    public string? AssignedBy { get; set; }
}
