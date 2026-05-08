using System.Diagnostics;
using System.Security.Claims;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;

namespace HrSystem.Web.Middleware;

public sealed class RequestAuditMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IRepository<AuditLog> auditLogs)
    {
        // Skip static assets to reduce noise.
        var path = context.Request.Path.Value ?? "";
        var shouldSkip =
            path.StartsWith("/lib/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase);

        var sw = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();

            if (!shouldSkip)
            {
                var user = context.User;
                int? userId = null;
                var userIdStr = user?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out var parsedId))
                {
                    userId = parsedId;
                }

                var username = user?.Identity?.IsAuthenticated == true ? user.Identity?.Name : null;

                var log = new AuditLog
                {
                    AtUtc = DateTimeOffset.UtcNow,
                    AppUserId = userId,
                    Username = username,
                    EventType = "Request",
                    Action = $"{context.Request.Method} {path}",
                    HttpMethod = context.Request.Method,
                    Path = path,
                    StatusCode = context.Response.StatusCode,
                    DurationMs = (int)sw.ElapsedMilliseconds,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers.UserAgent.ToString()
                };

                try
                {
                    await auditLogs.AddAsync(log);
                    await auditLogs.SaveChangesAsync();
                }
                catch
                {
                    // Never fail the request because audit logging failed.
                }
            }
        }
    }
}
