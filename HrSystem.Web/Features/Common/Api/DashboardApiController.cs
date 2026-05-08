using HrSystem.Application.Features.Common.Abstractions;
using HrSystem.Application.Features.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardApiController(IDashboardService dashboard) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummary>> Summary(CancellationToken cancellationToken) =>
        Ok(await dashboard.GetSummaryAsync(cancellationToken: cancellationToken));
}
