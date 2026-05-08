using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class ShiftsController(ICrudService<Shift> shifts) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await shifts.ListAsync(cancellationToken);
        return View(items);
    }

    public IActionResult Create() => View(new ShiftFormVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ShiftFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await shifts.CreateAsync(new Shift
        {
            Name = vm.Name,
            StartTime = vm.StartTime,
            EndTime = vm.EndTime,
            IsOvernight = vm.IsOvernight,
            IsFlexibleHours = vm.IsFlexibleHours,
            FlexInStartTime = vm.FlexInStartTime,
            FlexInEndTime = vm.FlexInEndTime,
            GraceMinutes = vm.GraceMinutes,
            RequiredWorkMinutes = vm.RequiredWorkMinutes
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await shifts.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new ShiftFormVm
        {
            Id = entity.Id,
            Name = entity.Name,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            IsOvernight = entity.IsOvernight,
            IsFlexibleHours = entity.IsFlexibleHours,
            FlexInStartTime = entity.FlexInStartTime,
            FlexInEndTime = entity.FlexInEndTime,
            GraceMinutes = entity.GraceMinutes,
            RequiredWorkMinutes = entity.RequiredWorkMinutes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ShiftFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await shifts.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = vm.Name;
        entity.StartTime = vm.StartTime;
        entity.EndTime = vm.EndTime;
        entity.IsOvernight = vm.IsOvernight;
        entity.IsFlexibleHours = vm.IsFlexibleHours;
        entity.FlexInStartTime = vm.FlexInStartTime;
        entity.FlexInEndTime = vm.FlexInEndTime;
        entity.GraceMinutes = vm.GraceMinutes;
        entity.RequiredWorkMinutes = vm.RequiredWorkMinutes;
        await shifts.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await shifts.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await shifts.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
