using HrSystem.Domain.Common;

namespace HrSystem.Domain.Entities;

public sealed class AppUser : BaseEntity
{
    public required string Username { get; set; }
    public string? DisplayName { get; set; }

    // Stored as a password-hash string (implementation in Application layer).
    public required string PasswordHash { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset? LastLoginAtUtc { get; set; }

    public List<AppUserRole> UserRoles { get; set; } = [];
}

public sealed class AppRole : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    public List<AppUserRole> UserRoles { get; set; } = [];
    public List<AppRolePermission> RolePermissions { get; set; } = [];
}

public sealed class AppPermission : BaseEntity
{
    public required string Code { get; set; }
    public string? Description { get; set; }

    public List<AppRolePermission> RolePermissions { get; set; } = [];
}

public sealed class AppUserRole : BaseEntity
{
    public int AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public int AppRoleId { get; set; }
    public AppRole? AppRole { get; set; }
}

public sealed class AppRolePermission : BaseEntity
{
    public int AppRoleId { get; set; }
    public AppRole? AppRole { get; set; }

    public int AppPermissionId { get; set; }
    public AppPermission? AppPermission { get; set; }
}
