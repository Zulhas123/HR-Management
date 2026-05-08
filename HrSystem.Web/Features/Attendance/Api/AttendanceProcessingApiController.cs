using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

public sealed class AttendanceProcessingRequest
{
    public DateOnly FromInclusive { get; set; }
    public DateOnly ToInclusive { get; set; }
    public bool RecomputeProcessed { get; set; }
}

[ApiController]
[Route("api/attendance/process")]
[Authorize]
public sealed class AttendanceProcessingApiController(IAttendanceProcessingService processing) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<IReadOnlyList<AttendanceRecord>>> Process(AttendanceProcessingRequest request, CancellationToken cancellationToken)
    {
        var result = await processing.ProcessAsync(
            request.FromInclusive,
            request.ToInclusive,
            request.RecomputeProcessed,
            cancellationToken);

        return Ok(result);
    }
}
