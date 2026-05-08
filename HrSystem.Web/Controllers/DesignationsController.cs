using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class DesignationsController(ICrudService<Designation> designations) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await designations.ListAsync(cancellationToken);
        return View(items);
    }

    public IActionResult Create() => View(new DesignationFormVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DesignationFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await designations.CreateAsync(new Designation { Name = vm.Name, Description = vm.Description }, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await designations.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new DesignationFormVm { Id = entity.Id, Name = entity.Name, Description = entity.Description });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DesignationFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await designations.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = vm.Name;
        entity.Description = vm.Description;
        await designations.UpdateAsync(entity, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await designations.GetByIdAsync(id, cancellationToken);
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
        await designations.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}

