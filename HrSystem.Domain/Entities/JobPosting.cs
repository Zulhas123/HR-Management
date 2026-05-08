using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class JobPosting : BaseEntity
{
    public required string Title { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public string? Description { get; set; }
    public bool IsOpen { get; set; } = true;
}

