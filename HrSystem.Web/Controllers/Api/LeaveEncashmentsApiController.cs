using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/leave-encashments")]
[Authorize]
public sealed class LeaveEncashmentsApiController(
    ICrudService<LeaveEncashmentRequest> encashments,
    ICrudService<LeaveType> leaveTypes,
    ILeaveBalanceService balances) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaveEncashmentRequest>>> List(CancellationToken cancellationToken) =>
        Ok(await encashments.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeaveEncashmentRequest>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await encashments.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveEncashmentRequest>> Create(LeaveEncashmentRequest request, CancellationToken cancellationToken)
    {
        var leaveType = await leaveTypes.GetByIdAsync(request.LeaveTypeId, cancellationToken);
        if (leaveType is null)
        {
            return BadRequest("Invalid LeaveTypeId.");
        }

        if (!leaveType.AllowEncashment)
        {
            return BadRequest("Encashment is not allowed for this leave type.");
        }

        if (leaveType.MaxEncashmentDaysPerYear is not null && request.DaysRequested > leaveType.MaxEncashmentDaysPerYear.Value)
        {
            return BadRequest($"Max encashment is {leaveType.MaxEncashmentDaysPerYear} days/year.");
        }

        request.Status = LeaveEncashmentStatus.Pending;
        request.RequestedAtUtc = DateTimeOffset.UtcNow;
        request.DecisionAtUtc = null;
        request.DecisionBy = null;
        request.DecisionNote = null;

        var created = await encashments.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] string? note, CancellationToken cancellationToken)
    {
        var entity = await encashments.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Status != LeaveEncashmentStatus.Pending)
        {
            return BadRequest("Only pending requests can be approved.");
        }

        try
        {
            await balances.ApplyEncashmentApprovedAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        entity.Status = LeaveEncashmentStatus.Approved;
        entity.DecisionAtUtc = DateTimeOffset.UtcNow;
        entity.DecisionBy = User?.Identity?.Name ?? "system";
        entity.DecisionNote = note;
        await encashments.UpdateAsync(entity, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] string? note, CancellationToken cancellationToken)
    {
        var entity = await encashments.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Status != LeaveEncashmentStatus.Pending)
        {
            return BadRequest("Only pending requests can be rejected.");
        }

        entity.Status = LeaveEncashmentStatus.Rejected;
        entity.DecisionAtUtc = DateTimeOffset.UtcNow;
        entity.DecisionBy = User?.Identity?.Name ?? "system";
        entity.DecisionNote = note;
        await encashments.UpdateAsync(entity, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/paid")]
    public async Task<IActionResult> MarkPaid(int id, CancellationToken cancellationToken)
    {
        var entity = await encashments.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Status != LeaveEncashmentStatus.Approved)
        {
            return BadRequest("Only approved requests can be marked as paid.");
        }

        entity.Status = LeaveEncashmentStatus.Paid;
        await encashments.UpdateAsync(entity, cancellationToken);
        return NoContent();
    }
}

