using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class ReligionsController(ICrudService<Religion> religions) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await religions.ListAsync(cancellationToken));

    public IActionResult Create() => View(new SimpleNameVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SimpleNameVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(vm);
        await religions.CreateAsync(new Religion { Name = vm.Name }, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await religions.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(new SimpleNameVm { Id = entity.Id, Name = entity.Name });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SimpleNameVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var entity = await religions.GetByIdAsync(id, cancellationToken);
        if (entity is null) return NotFound();

        entity.Name = vm.Name;
        await religions.UpdateAsync(entity, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await religions.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await religions.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
