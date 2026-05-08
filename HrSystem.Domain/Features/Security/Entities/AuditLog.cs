using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class AuditLog : BaseEntity
{
    public DateTimeOffset AtUtc { get; set; } = DateTimeOffset.UtcNow;

    public int? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public string? Username { get; set; }

    public string? EventType { get; set; } // e.g. LoginSuccess, Request
    public string? Action { get; set; } // e.g. "GET /api/employees"

    public string? HttpMethod { get; set; }
    public string? Path { get; set; }
    public int? StatusCode { get; set; }
    public int? DurationMs { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public string? Data { get; set; } // optional JSON payload
}
