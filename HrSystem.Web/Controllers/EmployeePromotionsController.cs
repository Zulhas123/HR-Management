using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class EmployeePromotionsController(
    ICrudService<Employee> employees,
    ICrudService<EmployeePromotion> promotions,
    ICrudService<Designation> designations) : Controller
{
    public async Task<IActionResult> Index(int employeeId, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var items = (await promotions.ListAsync(cancellationToken))
            .Where(p => p.EmployeeId == employeeId)
            .ToList();

        ViewData["Employee"] = employee;
        return View(items);
    }

    public async Task<IActionResult> Create(int employeeId, CancellationToken cancellationToken)
    {
        var vm = new EmployeePromotionFormVm { EmployeeId = employeeId };
        await PopulateDesignationsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeePromotionFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDesignationsAsync(vm, cancellationToken);
            return View(vm);
        }

        await promotions.CreateAsync(new EmployeePromotion
        {
            EmployeeId = vm.EmployeeId,
            EffectiveDate = DateOnly.FromDateTime(vm.EffectiveDate),
            FromDesignationId = vm.FromDesignationId,
            ToDesignationId = vm.ToDesignationId,
            Note = vm.Note
        }, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await promotions.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new EmployeePromotionFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EffectiveDate = entity.EffectiveDate.ToDateTime(TimeOnly.MinValue),
            FromDesignationId = entity.FromDesignationId,
            ToDesignationId = entity.ToDesignationId,
            Note = entity.Note
        };

        await PopulateDesignationsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeePromotionFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateDesignationsAsync(vm, cancellationToken);
            return View(vm);
        }

        var entity = await promotions.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.EffectiveDate = DateOnly.FromDateTime(vm.EffectiveDate);
        entity.FromDesignationId = vm.FromDesignationId;
        entity.ToDesignationId = vm.ToDesignationId;
        entity.Note = vm.Note;
        await promotions.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await promotions.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var entity = await promotions.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        await promotions.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index), new { employeeId = entity.EmployeeId });
    }

    private async Task PopulateDesignationsAsync(EmployeePromotionFormVm vm, CancellationToken cancellationToken)
    {
        vm.Designations = (await designations.ListAsync(cancellationToken))
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToList();
    }
}

