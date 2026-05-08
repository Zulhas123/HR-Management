using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class EmployeeTransfersController(
    ICrudService<Employee> employees,
    ICrudService<EmployeeTransfer> transfers,
    ICrudService<Department> departments) : Controller
{
    public async Task<IActionResult> Index(int employeeId, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var items = (await transfers.ListAsync(cancellationToken))
            .Where(t => t.EmployeeId == employeeId)
            .ToList();

        ViewData["Employee"] = employee;
        return View(items);
    }

    public async Task<IActionResult> Create(int employeeId, CancellationToken cancellationToken)
    {
        var vm = new EmployeeTransferFormVm { EmployeeId = employeeId };
        await PopulateDepartmentsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeTransferFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsAsync(vm, cancellationToken);
            return View(vm);
        }

        await transfers.CreateAsync(new EmployeeTransfer
        {
            EmployeeId = vm.EmployeeId,
            EffectiveDate = DateOnly.FromDateTime(vm.EffectiveDate),
            FromDepartmentId = vm.FromDepartmentId,
            ToDepartmentId = vm.ToDepartmentId,
            Note = vm.Note
        }, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await transfers.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new EmployeeTransferFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EffectiveDate = entity.EffectiveDate.ToDateTime(TimeOnly.MinValue),
            FromDepartmentId = entity.FromDepartmentId,
            ToDepartmentId = entity.ToDepartmentId,
            Note = entity.Note
        };

        await PopulateDepartmentsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeTransferFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateDepartmentsAsync(vm, cancellationToken);
            return View(vm);
        }

        var entity = await transfers.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.EffectiveDate = DateOnly.FromDateTime(vm.EffectiveDate);
        entity.FromDepartmentId = vm.FromDepartmentId;
        entity.ToDepartmentId = vm.ToDepartmentId;
        entity.Note = vm.Note;
        await transfers.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await transfers.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var entity = await transfers.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        await transfers.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index), new { employeeId = entity.EmployeeId });
    }

    private async Task PopulateDepartmentsAsync(EmployeeTransferFormVm vm, CancellationToken cancellationToken)
    {
        vm.Departments = (await departments.ListAsync(cancellationToken))
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToList();
    }
}
