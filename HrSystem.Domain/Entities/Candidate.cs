using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class Candidate : BaseEntity
{
    public required string FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CvUrl { get; set; }
    public string? Notes { get; set; }
}

