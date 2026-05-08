using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using HrSystem.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/login-history")]
[Authorize]
[Permission("audit.read")]
public sealed class LoginHistoryApiController(HrSystemDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LoginHistory>>> List(
        [FromQuery] int? userId,
        [FromQuery] int take = 200,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 1000);

        IQueryable<LoginHistory> query = db.LoginHistories.AsNoTracking().Include(x => x.AppUser);
        if (userId.HasValue)
        {
            query = query.Where(x => x.AppUserId == userId.Value);
        }

        var items = await query
            .OrderByDescending(x => x.LoggedInAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}
