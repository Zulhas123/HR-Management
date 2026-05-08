using HrSystem.Application.Abstractions;
using HrSystem.Application.Auth;
using HrSystem.Application.Security;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Services;

public sealed class AuthService(
    IUserRepository users,
    IRepository<LoginHistory> loginHistory,
    IRepository<AuditLog> auditLogs) : IAuthService
{
    public async Task<AuthResultDto?> AuthenticateAsync(
        string username,
        string password,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = await users.GetByUsernameAsync(username, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        if (!PasswordHasher.Verify(password, user.PasswordHash))
        {
            return null;
        }

        var roles = user.UserRoles
            .Where(ur => ur.AppRole is not null)
            .Select(ur => ur.AppRole!.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();

        var permissions = user.UserRoles
            .Where(ur => ur.AppRole is not null)
            .SelectMany(ur => ur.AppRole!.RolePermissions)
            .Where(rp => rp.AppPermission is not null)
            .Select(rp => rp.AppPermission!.Code)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();

        // Update last login + history + audit
        var now = DateTimeOffset.UtcNow;
        var update = await users.GetByIdAsync(user.Id, cancellationToken);
        if (update is not null)
        {
            update.LastLoginAtUtc = now;
            await users.UpdateAsync(update, cancellationToken);
            await users.SaveChangesAsync(cancellationToken);
        }

        await loginHistory.AddAsync(new LoginHistory
        {
            AppUserId = user.Id,
            LoggedInAtUtc = now,
            IpAddress = ipAddress,
            UserAgent = userAgent
        }, cancellationToken);

        await auditLogs.AddAsync(new AuditLog
        {
            AtUtc = now,
            AppUserId = user.Id,
            Username = user.Username,
            EventType = "LoginSuccess",
            Action = "Login",
            IpAddress = ipAddress,
            UserAgent = userAgent
        }, cancellationToken);

        await auditLogs.SaveChangesAsync(cancellationToken);

        return new AuthResultDto(
            user.Id,
            user.Username,
            user.DisplayName ?? user.Username,
            roles,
            permissions);
    }
}
