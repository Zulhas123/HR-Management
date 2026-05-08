using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class EmploymentTypesController(ICrudService<EmploymentType> employmentTypes) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await employmentTypes.ListAsync(cancellationToken);
        return View(items);
    }

    public IActionResult Create() => View(new EmploymentTypeFormVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmploymentTypeFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await employmentTypes.CreateAsync(new EmploymentType { Name = vm.Name }, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await employmentTypes.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new EmploymentTypeFormVm { Id = entity.Id, Name = entity.Name });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmploymentTypeFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await employmentTypes.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = vm.Name;
        await employmentTypes.UpdateAsync(entity, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await employmentTypes.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await employmentTypes.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}

