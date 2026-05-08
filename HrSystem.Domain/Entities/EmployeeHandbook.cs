using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeHandbook : BaseEntity
{
    public required string Title { get; set; }
    public required string FilePath { get; set; }
    public DateTimeOffset PublishedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}

