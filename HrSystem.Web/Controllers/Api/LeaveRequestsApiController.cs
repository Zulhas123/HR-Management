using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/leave-requests")]
[Authorize]
public sealed class LeaveRequestsApiController(
    ICrudService<LeaveRequest> leaveRequests,
    ICrudService<LeaveType> leaveTypes,
    ILeaveCalendarService calendar,
    ILeaveBalanceService balances) : ControllerBase
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

        var leaveType = await leaveTypes.GetByIdAsync(request.LeaveTypeId, cancellationToken);
        if (leaveType is null)
        {
            return BadRequest("Invalid LeaveTypeId.");
        }

        request.TotalDays = await calendar.CalculateChargeableDaysAsync(request.StartDate, request.EndDate, leaveType, cancellationToken);
        if (request.TotalDays <= 0)
        {
            return BadRequest("Selected dates contain no chargeable leave days (weekends/holidays excluded).");
        }
        request.ApprovalLevelsRequired = Math.Max(1, leaveType.ApprovalLevelsRequired);
        request.ApprovalLevelsApproved = 0;

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

        var existing = await leaveRequests.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (existing.Status != LeaveRequestStatus.Pending || existing.ApprovalLevelsApproved > 0)
        {
            return BadRequest("Only pending requests with no approvals can be edited.");
        }

        if (request.EndDate < request.StartDate)
        {
            return BadRequest("EndDate must be on or after StartDate.");
        }

        var leaveType = await leaveTypes.GetByIdAsync(request.LeaveTypeId, cancellationToken);
        if (leaveType is null)
        {
            return BadRequest("Invalid LeaveTypeId.");
        }

        existing.EmployeeId = request.EmployeeId;
        existing.LeaveTypeId = request.LeaveTypeId;
        existing.StartDate = request.StartDate;
        existing.EndDate = request.EndDate;
        existing.TotalDays = await calendar.CalculateChargeableDaysAsync(request.StartDate, request.EndDate, leaveType, cancellationToken);
        if (existing.TotalDays <= 0)
        {
            return BadRequest("Selected dates contain no chargeable leave days (weekends/holidays excluded).");
        }
        existing.Reason = request.Reason;
        existing.ApprovalLevelsRequired = Math.Max(1, leaveType.ApprovalLevelsRequired);
        existing.ApprovalLevelsApproved = 0;

        await leaveRequests.UpdateAsync(existing, cancellationToken);
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

        var nextLevel = entity.ApprovalLevelsApproved + 1;
        if (nextLevel > Math.Max(1, entity.ApprovalLevelsRequired))
        {
            return BadRequest("Approval levels already completed.");
        }

        entity.ApprovalSteps.Add(new LeaveApprovalStep
        {
            LeaveRequestId = entity.Id,
            Level = nextLevel,
            Decision = LeaveApprovalDecision.Approved,
            DecidedAtUtc = DateTimeOffset.UtcNow,
            DecidedBy = User?.Identity?.Name ?? "system",
            Note = note
        });

        entity.ApprovalLevelsApproved = nextLevel;

        if (entity.ApprovalLevelsApproved >= Math.Max(1, entity.ApprovalLevelsRequired))
        {
            try
            {
                await balances.ApplyApprovedLeaveAsync(entity, cancellationToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            entity.Status = LeaveRequestStatus.Approved;
            entity.DecisionAtUtc = DateTime.UtcNow;
            entity.DecisionBy = User?.Identity?.Name ?? "system";
            entity.DecisionNote = note;
        }

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

        var nextLevel = entity.ApprovalLevelsApproved + 1;
        if (nextLevel > Math.Max(1, entity.ApprovalLevelsRequired))
        {
            nextLevel = Math.Max(1, entity.ApprovalLevelsRequired);
        }

        entity.ApprovalSteps.Add(new LeaveApprovalStep
        {
            LeaveRequestId = entity.Id,
            Level = nextLevel,
            Decision = LeaveApprovalDecision.Rejected,
            DecidedAtUtc = DateTimeOffset.UtcNow,
            DecidedBy = User?.Identity?.Name ?? "system",
            Note = note
        });

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
