using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class HolidaysController(ICrudService<Holiday> holidays) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await holidays.ListAsync(cancellationToken);
        return View(items.OrderBy(x => x.Date).ToList());
    }

    public IActionResult Create() => View(new HolidayFormVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HolidayFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await holidays.CreateAsync(new Holiday
        {
            Date = DateOnly.FromDateTime(vm.Date),
            Name = vm.Name,
            IsOptional = vm.IsOptional
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await holidays.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new HolidayFormVm
        {
            Id = entity.Id,
            Date = entity.Date.ToDateTime(TimeOnly.MinValue),
            Name = entity.Name,
            IsOptional = entity.IsOptional
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HolidayFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await holidays.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Date = DateOnly.FromDateTime(vm.Date);
        entity.Name = vm.Name;
        entity.IsOptional = vm.IsOptional;
        await holidays.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await holidays.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await holidays.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
