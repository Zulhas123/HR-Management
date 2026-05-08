using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeDocument : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public required string DocumentName { get; set; }
    public string? DocumentType { get; set; }

    public required string StoredPath { get; set; }
    public string? OriginalFileName { get; set; }

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}

