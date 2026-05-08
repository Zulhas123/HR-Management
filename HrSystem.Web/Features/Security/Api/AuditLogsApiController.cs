using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using HrSystem.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/audit-logs")]
[Authorize]
[Permission("audit.read")]
public sealed class AuditLogsApiController(HrSystemDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditLog>>> List(
        [FromQuery] DateTimeOffset? fromUtc,
        [FromQuery] DateTimeOffset? toUtc,
        [FromQuery] string? eventType,
        [FromQuery] string? username,
        [FromQuery] int take = 200,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 1000);

        var query = db.AuditLogs.AsNoTracking();

        if (fromUtc.HasValue)
        {
            query = query.Where(x => x.AtUtc >= fromUtc.Value);
        }
        if (toUtc.HasValue)
        {
            query = query.Where(x => x.AtUtc <= toUtc.Value);
        }
        if (!string.IsNullOrWhiteSpace(eventType))
        {
            query = query.Where(x => x.EventType == eventType);
        }
        if (!string.IsNullOrWhiteSpace(username))
        {
            query = query.Where(x => x.Username == username);
        }

        var items = await query
            .OrderByDescending(x => x.AtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}
