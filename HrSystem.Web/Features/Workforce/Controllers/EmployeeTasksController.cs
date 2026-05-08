using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class EmployeeTasksController(
    ICrudService<EmployeeTask> tasks,
    ICrudService<Employee> employees) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await tasks.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new EmployeeTaskFormVm();
        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeTaskFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        await tasks.CreateAsync(new EmployeeTask
        {
            EmployeeId = vm.EmployeeId,
            Title = vm.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(vm.Description) ? null : vm.Description.Trim(),
            Priority = vm.Priority,
            Status = vm.Status,
            AssignedDate = DateOnly.FromDateTime(vm.AssignedDate),
            DueDate = vm.DueDate.HasValue ? DateOnly.FromDateTime(vm.DueDate.Value) : null,
            CompletedDate = vm.CompletedDate.HasValue ? DateOnly.FromDateTime(vm.CompletedDate.Value) : null,
            EstimatedMinutes = vm.EstimatedMinutes,
            ActualMinutes = vm.ActualMinutes,
            AssignedBy = string.IsNullOrWhiteSpace(vm.AssignedBy) ? null : vm.AssignedBy.Trim(),
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await tasks.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new EmployeeTaskFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Title = entity.Title,
            Description = entity.Description,
            Priority = entity.Priority,
            Status = entity.Status,
            AssignedDate = entity.AssignedDate.ToDateTime(TimeOnly.MinValue),
            DueDate = entity.DueDate?.ToDateTime(TimeOnly.MinValue),
            CompletedDate = entity.CompletedDate?.ToDateTime(TimeOnly.MinValue),
            EstimatedMinutes = entity.EstimatedMinutes,
            ActualMinutes = entity.ActualMinutes,
            AssignedBy = entity.AssignedBy
        };

        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeTaskFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        var entity = await tasks.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.EmployeeId = vm.EmployeeId;
        entity.Title = vm.Title.Trim();
        entity.Description = string.IsNullOrWhiteSpace(vm.Description) ? null : vm.Description.Trim();
        entity.Priority = vm.Priority;
        entity.Status = vm.Status;
        entity.AssignedDate = DateOnly.FromDateTime(vm.AssignedDate);
        entity.DueDate = vm.DueDate.HasValue ? DateOnly.FromDateTime(vm.DueDate.Value) : null;
        entity.CompletedDate = vm.CompletedDate.HasValue ? DateOnly.FromDateTime(vm.CompletedDate.Value) : null;
        entity.EstimatedMinutes = vm.EstimatedMinutes;
        entity.ActualMinutes = vm.ActualMinutes;
        entity.AssignedBy = string.IsNullOrWhiteSpace(vm.AssignedBy) ? null : vm.AssignedBy.Trim();

        await tasks.UpdateAsync(entity, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await tasks.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await tasks.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookupsAsync(EmployeeTaskFormVm vm, CancellationToken cancellationToken)
    {
        var employeeItems = await employees.ListAsync(cancellationToken);
        vm.Employees = new SelectList(employeeItems, nameof(Employee.Id), nameof(Employee.EmployeeCode), vm.EmployeeId);
    }
}
