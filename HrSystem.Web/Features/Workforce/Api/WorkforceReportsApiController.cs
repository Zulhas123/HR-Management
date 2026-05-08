using HrSystem.Application.Abstractions;
using HrSystem.Application.Workforce;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/workforce/reports")]
[Authorize]
public sealed class WorkforceReportsApiController(IWorkforceReportingService reporting) : ControllerBase
{
    [HttpGet("productivity")]
    public async Task<ActionResult<WorkforceProductivityReportDto>> Productivity(
        [FromQuery] DateOnly fromInclusive,
        [FromQuery] DateOnly toInclusive,
        [FromQuery] int? departmentId,
        CancellationToken cancellationToken)
    {
        var result = await reporting.GetProductivityReportAsync(fromInclusive, toInclusive, departmentId, cancellationToken);
        return Ok(result);
    }
}
