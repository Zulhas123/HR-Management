using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class WeekendConfigurationController(ICrudService<WeekendConfiguration> weekends) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var current = (await weekends.ListAsync(cancellationToken)).FirstOrDefault();
        return View(current);
    }

    public async Task<IActionResult> Edit(CancellationToken cancellationToken)
    {
        var entity = (await weekends.ListAsync(cancellationToken)).FirstOrDefault();
        entity ??= await weekends.CreateAsync(new WeekendConfiguration { Friday = true, Saturday = true }, cancellationToken);

        return View(new WeekendConfigurationVm
        {
            Id = entity.Id,
            Sunday = entity.Sunday,
            Monday = entity.Monday,
            Tuesday = entity.Tuesday,
            Wednesday = entity.Wednesday,
            Thursday = entity.Thursday,
            Friday = entity.Friday,
            Saturday = entity.Saturday
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(WeekendConfigurationVm vm, CancellationToken cancellationToken)
    {
        var entity = await weekends.GetByIdAsync(vm.Id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Sunday = vm.Sunday;
        entity.Monday = vm.Monday;
        entity.Tuesday = vm.Tuesday;
        entity.Wednesday = vm.Wednesday;
        entity.Thursday = vm.Thursday;
        entity.Friday = vm.Friday;
        entity.Saturday = vm.Saturday;
        await weekends.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }
}

