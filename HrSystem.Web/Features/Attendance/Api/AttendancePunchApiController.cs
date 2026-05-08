using HrSystem.Application.Abstractions;
using HrSystem.Application.Attendance;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/attendance/punch")]
[Authorize]
public sealed class AttendancePunchApiController(IAttendancePunchService punches) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AttendanceRecord>> Punch(AttendancePunchRequest request, CancellationToken cancellationToken)
    {
        var record = await punches.PunchAsync(request, cancellationToken);
        return Ok(record);
    }
}
