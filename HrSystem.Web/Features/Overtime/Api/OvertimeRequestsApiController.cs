using HrSystem.Application.Abstractions;
using HrSystem.Application.Overtime;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

public sealed class OvertimeAutoGenerateRequest
{
    public DateOnly FromInclusive { get; set; }
    public DateOnly ToInclusive { get; set; }
    public bool CreateIfMissing { get; set; } = true;
}

[ApiController]
[Route("api/overtime-requests")]
[Authorize]
public sealed class OvertimeRequestsApiController(
    ICrudService<OvertimeRequest> overtimeRequests,
    IOvertimeService overtime,
    ILeaveCalendarService calendar) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OvertimeRequest>>> List(CancellationToken cancellationToken) =>
        Ok(await overtimeRequests.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OvertimeRequest>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await overtimeRequests.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<OvertimeRequest>> Create(OvertimeRequest request, CancellationToken cancellationToken)
    {
        var policy = await overtime.GetActivePolicyAsync(cancellationToken);

        var isHoliday = await calendar.IsHolidayAsync(request.Date, cancellationToken);
        request.IsHoliday = isHoliday;
        request.PayMultiplier = isHoliday ? policy.HolidayMultiplier : policy.NormalMultiplier;

        request.CalculatedMinutes = Math.Max(0, request.CalculatedMinutes);
        request.RequestedMinutes = Math.Max(0, request.RequestedMinutes);

        if (request.CalculatedMinutes <= 0)
        {
            request.CalculatedMinutes = request.RequestedMinutes;
        }

        request.ApprovalLevelsRequired = Math.Max(1, policy.ApprovalLevelsRequired);
        request.ApprovalLevelsApproved = 0;
        request.Status = OvertimeRequestStatus.Pending;
        request.DecisionAtUtc = null;
        request.DecisionBy = null;
        request.DecisionNote = null;
        request.ApprovedMinutes = null;

        // Avoid over-posting navigation properties
        request.Employee = null;
        request.AttendanceRecord = null;
        request.ApprovalSteps = [];

        var created = await overtimeRequests.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, OvertimeRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        var existing = await overtimeRequests.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (existing.Status != OvertimeRequestStatus.Pending || existing.ApprovalLevelsApproved > 0)
        {
            return BadRequest("Only pending requests with no approvals can be edited.");
        }

        var policy = await overtime.GetActivePolicyAsync(cancellationToken);

        existing.EmployeeId = request.EmployeeId;
        existing.Date = request.Date;
        existing.AttendanceRecordId = request.AttendanceRecordId;
        existing.Reason = request.Reason;

        var isHoliday = await calendar.IsHolidayAsync(existing.Date, cancellationToken);
        existing.IsHoliday = isHoliday;
        existing.PayMultiplier = isHoliday ? policy.HolidayMultiplier : policy.NormalMultiplier;

        existing.CalculatedMinutes = Math.Max(0, request.CalculatedMinutes);
        existing.RequestedMinutes = Math.Max(0, request.RequestedMinutes);
        if (existing.CalculatedMinutes <= 0)
        {
            existing.CalculatedMinutes = existing.RequestedMinutes;
        }

        existing.ApprovalLevelsRequired = Math.Max(1, policy.ApprovalLevelsRequired);
        existing.ApprovalLevelsApproved = 0;

        await overtimeRequests.UpdateAsync(existing, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] string? note, CancellationToken cancellationToken)
    {
        await overtime.ApproveAsync(id, User?.Identity?.Name ?? "system", note, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] string? note, CancellationToken cancellationToken)
    {
        await overtime.RejectAsync(id, User?.Identity?.Name ?? "system", note, cancellationToken);
        return NoContent();
    }

    [HttpPost("auto-generate")]
    public async Task<ActionResult<OvertimeAutoGenerationResultDto>> AutoGenerate(
        OvertimeAutoGenerateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await overtime.AutoGenerateFromAttendanceAsync(
            request.FromInclusive,
            request.ToInclusive,
            request.CreateIfMissing,
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await overtimeRequests.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
