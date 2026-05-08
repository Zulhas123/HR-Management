using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class LeaveEncashmentsController(
    ICrudService<LeaveEncashmentRequest> encashments,
    ICrudService<Employee> employees,
    ICrudService<LeaveType> leaveTypes,
    ILeaveBalanceService balances) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await encashments.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new LeaveEncashmentRequestFormVm();
        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveEncashmentRequestFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        var leaveType = await leaveTypes.GetByIdAsync(vm.LeaveTypeId, cancellationToken);
        if (leaveType is null)
        {
            return BadRequest("Invalid leave type.");
        }

        if (!leaveType.AllowEncashment)
        {
            ModelState.AddModelError(nameof(vm.LeaveTypeId), "Encashment is not allowed for this leave type.");
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        if (leaveType.MaxEncashmentDaysPerYear is not null && vm.DaysRequested > leaveType.MaxEncashmentDaysPerYear.Value)
        {
            ModelState.AddModelError(nameof(vm.DaysRequested), $"Max encashment is {leaveType.MaxEncashmentDaysPerYear} days/year.");
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        await encashments.CreateAsync(new LeaveEncashmentRequest
        {
            EmployeeId = vm.EmployeeId,
            LeaveTypeId = vm.LeaveTypeId,
            Year = vm.Year,
            DaysRequested = vm.DaysRequested,
            Status = LeaveEncashmentStatus.Pending
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        var entity = await encashments.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(new LeaveDecisionVm { Id = entity.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(LeaveDecisionVm vm, CancellationToken cancellationToken)
    {
        var entity = await encashments.GetByIdAsync(vm.Id, cancellationToken);
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
        entity.DecisionNote = vm.DecisionNote;
        await encashments.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Reject(int id, CancellationToken cancellationToken)
    {
        var entity = await encashments.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(new LeaveDecisionVm { Id = entity.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(LeaveDecisionVm vm, CancellationToken cancellationToken)
    {
        var entity = await encashments.GetByIdAsync(vm.Id, cancellationToken);
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
        entity.DecisionNote = vm.DecisionNote;
        await encashments.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await encashments.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await encashments.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookupsAsync(LeaveEncashmentRequestFormVm vm, CancellationToken cancellationToken)
    {
        vm.Employees = (await employees.ListAsync(cancellationToken))
            .Select(e => new SelectListItem($"{e.EmployeeCode} - {e.FirstName} {e.LastName}", e.Id.ToString()))
            .ToList();

        vm.LeaveTypes = (await leaveTypes.ListAsync(cancellationToken))
            .Where(t => t.AllowEncashment)
            .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
            .ToList();
    }
}
