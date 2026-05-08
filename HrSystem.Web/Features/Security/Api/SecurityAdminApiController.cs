using HrSystem.Application.Security;
using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using HrSystem.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Web.Controllers.Api;

public sealed record CreateUserRequest(string Username, string Password, string? DisplayName, bool IsActive = true);
public sealed record UpdateUserRequest(string? DisplayName, bool? IsActive);
public sealed record SetUserPasswordRequest(string Password);
public sealed record SetUserRolesRequest(IReadOnlyList<string> Roles);
public sealed record CreateRoleRequest(string Name, string? Description);
public sealed record UpdateRoleRequest(string? Name, string? Description);
public sealed record SetRolePermissionsRequest(IReadOnlyList<string> Permissions);
public sealed record CreatePermissionRequest(string Code, string? Description);
public sealed record UpdatePermissionRequest(string? Code, string? Description);

[ApiController]
[Route("api/security")]
[Authorize]
[Permission("security.manage")]
public sealed class SecurityAdminApiController(HrSystemDbContext db) : ControllerBase
{
    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> ListUsers(CancellationToken cancellationToken) =>
        Ok(await db.AppUsers.AsNoTracking().OrderBy(x => x.Username).ToListAsync(cancellationToken));

    [HttpGet("users/{id:int}")]
    public async Task<ActionResult<AppUser>> GetUser(int id, CancellationToken cancellationToken)
    {
        var user = await db.AppUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost("users")]
    public async Task<ActionResult<AppUser>> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and Password are required.");
        }

        var exists = await db.AppUsers.AnyAsync(x => x.Username == request.Username, cancellationToken);
        if (exists)
        {
            return Conflict("Username already exists.");
        }

        var user = new AppUser
        {
            Username = request.Username.Trim(),
            DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? request.Username.Trim() : request.DisplayName.Trim(),
            PasswordHash = PasswordHasher.Hash(request.Password),
            IsActive = request.IsActive
        };

        db.AppUsers.Add(user);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(user);
    }

    [HttpPut("users/{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        if (request.DisplayName is not null)
        {
            user.DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? null : request.DisplayName.Trim();
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("users/{id:int}/set-password")]
    public async Task<IActionResult> SetUserPassword(int id, SetUserPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Password is required.");
        }

        user.PasswordHash = PasswordHasher.Hash(request.Password);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        db.AppUserRoles.RemoveRange(db.AppUserRoles.Where(x => x.AppUserId == id));
        db.LoginHistories.RemoveRange(db.LoginHistories.Where(x => x.AppUserId == id));
        db.AuditLogs.RemoveRange(db.AuditLogs.Where(x => x.AppUserId == id));
        db.AppUsers.Remove(user);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("users/{id:int}/set-roles")]
    public async Task<IActionResult> SetUserRoles(int id, SetUserRolesRequest request, CancellationToken cancellationToken)
    {
        var user = await db.AppUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var roleNames = (request.Roles ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var roles = await db.AppRoles.Where(r => roleNames.Contains(r.Name)).ToListAsync(cancellationToken);

        db.AppUserRoles.RemoveRange(db.AppUserRoles.Where(x => x.AppUserId == id));
        foreach (var role in roles)
        {
            db.AppUserRoles.Add(new AppUserRole { AppUserId = id, AppRoleId = role.Id });
        }

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyList<AppRole>>> ListRoles(CancellationToken cancellationToken) =>
        Ok(await db.AppRoles.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken));

    [HttpGet("roles/{id:int}")]
    public async Task<ActionResult<AppRole>> GetRole(int id, CancellationToken cancellationToken)
    {
        var role = await db.AppRoles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost("roles")]
    public async Task<ActionResult<AppRole>> CreateRole(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Name is required.");
        }

        var exists = await db.AppRoles.AnyAsync(x => x.Name == request.Name, cancellationToken);
        if (exists)
        {
            return Conflict("Role already exists.");
        }

        var role = new AppRole { Name = request.Name.Trim(), Description = request.Description };
        db.AppRoles.Add(role);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(role);
    }

    [HttpPut("roles/{id:int}")]
    public async Task<IActionResult> UpdateRole(int id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = await db.AppRoles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (role is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Name) && !string.Equals(role.Name, request.Name.Trim(), StringComparison.Ordinal))
        {
            var newName = request.Name.Trim();
            var exists = await db.AppRoles.AnyAsync(x => x.Id != id && x.Name == newName, cancellationToken);
            if (exists)
            {
                return Conflict("Role name already exists.");
            }

            role.Name = newName;
        }

        if (request.Description is not null)
        {
            role.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        }

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("roles/{id:int}/set-permissions")]
    public async Task<IActionResult> SetRolePermissions(int id, SetRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        var role = await db.AppRoles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (role is null)
        {
            return NotFound();
        }

        var codes = (request.Permissions ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var perms = await db.AppPermissions.Where(p => codes.Contains(p.Code)).ToListAsync(cancellationToken);

        db.AppRolePermissions.RemoveRange(db.AppRolePermissions.Where(x => x.AppRoleId == id));
        foreach (var p in perms)
        {
            db.AppRolePermissions.Add(new AppRolePermission { AppRoleId = id, AppPermissionId = p.Id });
        }

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("roles/{id:int}")]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cancellationToken)
    {
        var role = await db.AppRoles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (role is null)
        {
            return NotFound();
        }

        db.AppRolePermissions.RemoveRange(db.AppRolePermissions.Where(x => x.AppRoleId == id));
        db.AppUserRoles.RemoveRange(db.AppUserRoles.Where(x => x.AppRoleId == id));
        db.AppRoles.Remove(role);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("permissions")]
    public async Task<ActionResult<IReadOnlyList<AppPermission>>> ListPermissions(CancellationToken cancellationToken) =>
        Ok(await db.AppPermissions.AsNoTracking().OrderBy(x => x.Code).ToListAsync(cancellationToken));

    [HttpGet("permissions/{id:int}")]
    public async Task<ActionResult<AppPermission>> GetPermission(int id, CancellationToken cancellationToken)
    {
        var perm = await db.AppPermissions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return perm is null ? NotFound() : Ok(perm);
    }

    [HttpPost("permissions")]
    public async Task<ActionResult<AppPermission>> CreatePermission(CreatePermissionRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest("Code is required.");
        }

        var exists = await db.AppPermissions.AnyAsync(x => x.Code == request.Code, cancellationToken);
        if (exists)
        {
            return Conflict("Permission already exists.");
        }

        var perm = new AppPermission { Code = request.Code.Trim(), Description = request.Description };
        db.AppPermissions.Add(perm);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(perm);
    }

    [HttpPut("permissions/{id:int}")]
    public async Task<IActionResult> UpdatePermission(int id, UpdatePermissionRequest request, CancellationToken cancellationToken)
    {
        var perm = await db.AppPermissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (perm is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Code) && !string.Equals(perm.Code, request.Code.Trim(), StringComparison.Ordinal))
        {
            var newCode = request.Code.Trim();
            var exists = await db.AppPermissions.AnyAsync(x => x.Id != id && x.Code == newCode, cancellationToken);
            if (exists)
            {
                return Conflict("Permission code already exists.");
            }

            perm.Code = newCode;
        }

        if (request.Description is not null)
        {
            perm.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        }

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("permissions/{id:int}")]
    public async Task<IActionResult> DeletePermission(int id, CancellationToken cancellationToken)
    {
        var perm = await db.AppPermissions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (perm is null)
        {
            return NotFound();
        }

        db.AppRolePermissions.RemoveRange(db.AppRolePermissions.Where(x => x.AppPermissionId == id));
        db.AppPermissions.Remove(perm);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
