using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class LoginHistory : BaseEntity
{
    public DateTimeOffset LoggedInAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public int AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
