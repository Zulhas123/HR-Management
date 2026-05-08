using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/leave-requests")]
[Authorize]
public sealed class LeaveRequestsApiController(ICrudService<LeaveRequest> leaveRequests) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaveRequest>>> List(CancellationToken cancellationToken) =>
        Ok(await leaveRequests.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeaveRequest>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await leaveRequests.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequest>> Create(LeaveRequest request, CancellationToken cancellationToken)
    {
        if (request.EndDate < request.StartDate)
        {
            return BadRequest("EndDate must be on or after StartDate.");
        }

        request.TotalDays = request.TotalDays <= 0
            ? (decimal)(request.EndDate.DayNumber - request.StartDate.DayNumber + 1)
            : request.TotalDays;

        request.Status = LeaveRequestStatus.Pending;
        request.DecisionAtUtc = null;
        request.DecisionBy = null;
        request.DecisionNote = null;

        var created = await leaveRequests.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LeaveRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        await leaveRequests.UpdateAsync(request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] string? note, CancellationToken cancellationToken)
    {
        var entity = await leaveRequests.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Status != LeaveRequestStatus.Pending)
        {
            return BadRequest("Only pending requests can be approved.");
        }

        entity.Status = LeaveRequestStatus.Approved;
        entity.DecisionAtUtc = DateTime.UtcNow;
        entity.DecisionBy = User?.Identity?.Name ?? "system";
        entity.DecisionNote = note;
        await leaveRequests.UpdateAsync(entity, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] string? note, CancellationToken cancellationToken)
    {
        var entity = await leaveRequests.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Status != LeaveRequestStatus.Pending)
        {
            return BadRequest("Only pending requests can be rejected.");
        }

        entity.Status = LeaveRequestStatus.Rejected;
        entity.DecisionAtUtc = DateTime.UtcNow;
        entity.DecisionBy = User?.Identity?.Name ?? "system";
        entity.DecisionNote = note;
        await leaveRequests.UpdateAsync(entity, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await leaveRequests.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

