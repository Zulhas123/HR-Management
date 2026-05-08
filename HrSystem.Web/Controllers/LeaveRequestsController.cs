using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class LeaveRequestsController(
    ICrudService<LeaveRequest> leaveRequests,
    ICrudService<Employee> employees,
    ICrudService<LeaveType> leaveTypes) : Controller
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

        var totalDays = (decimal)(end.DayNumber - start.DayNumber + 1);

        await leaveRequests.CreateAsync(new LeaveRequest
        {
            EmployeeId = vm.EmployeeId,
            LeaveTypeId = vm.LeaveTypeId,
            StartDate = start,
            EndDate = end,
            TotalDays = totalDays,
            Reason = vm.Reason,
            Status = LeaveRequestStatus.Pending
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
        entity.TotalDays = (decimal)(end.DayNumber - start.DayNumber + 1);
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

        entity.Status = LeaveRequestStatus.Approved;
        entity.DecisionAtUtc = DateTime.UtcNow;
        entity.DecisionBy = User?.Identity?.Name ?? "system";
        entity.DecisionNote = vm.DecisionNote;
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

