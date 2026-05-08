using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class LeaveTypesController(ICrudService<LeaveType> leaveTypes) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await leaveTypes.ListAsync(cancellationToken);
        return View(items);
    }

    public IActionResult Create() => View(new LeaveTypeFormVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveTypeFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await leaveTypes.CreateAsync(new LeaveType
        {
            Name = vm.Name,
            Description = vm.Description,
            DefaultAnnualAllocation = vm.DefaultAnnualAllocation,
            IsPaid = vm.IsPaid
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await leaveTypes.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new LeaveTypeFormVm
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            DefaultAnnualAllocation = entity.DefaultAnnualAllocation,
            IsPaid = entity.IsPaid
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LeaveTypeFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await leaveTypes.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = vm.Name;
        entity.Description = vm.Description;
        entity.DefaultAnnualAllocation = vm.DefaultAnnualAllocation;
        entity.IsPaid = vm.IsPaid;
        await leaveTypes.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await leaveTypes.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await leaveTypes.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}

