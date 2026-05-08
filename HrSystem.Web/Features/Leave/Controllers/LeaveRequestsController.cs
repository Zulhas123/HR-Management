using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class LeaveRequestsController(
    ICrudService<LeaveRequest> leaveRequests,
    ICrudService<Employee> employees,
    ICrudService<LeaveType> leaveTypes,
    ILeaveCalendarService calendar,
    ILeaveBalanceService balances) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await leaveRequests.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new LeaveRequestFormVm();
        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveRequestFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        var start = DateOnly.FromDateTime(vm.StartDate);
        var end = DateOnly.FromDateTime(vm.EndDate);
        if (end < start)
        {
            ModelState.AddModelError(nameof(vm.EndDate), "End date must be on or after start date.");
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        var leaveType = await leaveTypes.GetByIdAsync(vm.LeaveTypeId, cancellationToken);
        if (leaveType is null)
        {
            return BadRequest("Invalid leave type.");
        }

        var totalDays = await calendar.CalculateChargeableDaysAsync(start, end, leaveType, cancellationToken);
        if (totalDays <= 0)
        {
            ModelState.AddModelError(nameof(vm.EndDate), "Selected dates contain no chargeable leave days (weekends/holidays excluded).");
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        await leaveRequests.CreateAsync(new LeaveRequest
        {
            EmployeeId = vm.EmployeeId,
            LeaveTypeId = vm.LeaveTypeId,
            StartDate = start,
            EndDate = end,
            TotalDays = totalDays,
            Reason = vm.Reason,
            Status = LeaveRequestStatus.Pending,
            ApprovalLevelsRequired = Math.Max(1, leaveType.ApprovalLevelsRequired),
            ApprovalLevelsApproved = 0
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await leaveRequests.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Status != LeaveRequestStatus.Pending)
        {
            return BadRequest("Only pending requests can be edited.");
        }

        if (entity.ApprovalLevelsApproved > 0)
        {
            return BadRequest("Requests cannot be edited after approvals have started.");
        }

        var vm = new LeaveRequestFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            LeaveTypeId = entity.LeaveTypeId,
            StartDate = entity.StartDate.ToDateTime(TimeOnly.MinValue),
            EndDate = entity.EndDate.ToDateTime(TimeOnly.MinValue),
            Reason = entity.Reason
        };

        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LeaveRequestFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        var entity = await leaveRequests.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Status != LeaveRequestStatus.Pending)
        {
            return BadRequest("Only pending requests can be edited.");
        }

        if (entity.ApprovalLevelsApproved > 0)
        {
            return BadRequest("Requests cannot be edited after approvals have started.");
        }

        var start = DateOnly.FromDateTime(vm.StartDate);
        var end = DateOnly.FromDateTime(vm.EndDate);
        if (end < start)
        {
            ModelState.AddModelError(nameof(vm.EndDate), "End date must be on or after start date.");
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        entity.EmployeeId = vm.EmployeeId;
        entity.LeaveTypeId = vm.LeaveTypeId;
        entity.StartDate = start;
        entity.EndDate = end;
        var leaveType = await leaveTypes.GetByIdAsync(vm.LeaveTypeId, cancellationToken);
        if (leaveType is null)
        {
            return BadRequest("Invalid leave type.");
        }

        entity.TotalDays = await calendar.CalculateChargeableDaysAsync(start, end, leaveType, cancellationToken);
        if (entity.TotalDays <= 0)
        {
            ModelState.AddModelError(nameof(vm.EndDate), "Selected dates contain no chargeable leave days (weekends/holidays excluded).");
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }
        entity.Reason = vm.Reason;
        await leaveRequests.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        var entity = await leaveRequests.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(new LeaveDecisionVm { Id = entity.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(LeaveDecisionVm vm, CancellationToken cancellationToken)
    {
        var entity = await leaveRequests.GetByIdAsync(vm.Id, cancellationToken);
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
            Note = vm.DecisionNote
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
            entity.DecisionNote = vm.DecisionNote;
        }

        await leaveRequests.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Reject(int id, CancellationToken cancellationToken)
    {
        var entity = await leaveRequests.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(new LeaveDecisionVm { Id = entity.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(LeaveDecisionVm vm, CancellationToken cancellationToken)
    {
        var entity = await leaveRequests.GetByIdAsync(vm.Id, cancellationToken);
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
            Note = vm.DecisionNote
        });

        entity.Status = LeaveRequestStatus.Rejected;
        entity.DecisionAtUtc = DateTime.UtcNow;
        entity.DecisionBy = User?.Identity?.Name ?? "system";
        entity.DecisionNote = vm.DecisionNote;

        await leaveRequests.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await leaveRequests.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await leaveRequests.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookupsAsync(LeaveRequestFormVm vm, CancellationToken cancellationToken)
    {
        vm.Employees = (await employees.ListAsync(cancellationToken))
            .Select(e => new SelectListItem($"{e.EmployeeCode} - {e.FirstName} {e.LastName}", e.Id.ToString()))
            .ToList();

        vm.LeaveTypes = (await leaveTypes.ListAsync(cancellationToken))
            .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
            .ToList();
    }
}
