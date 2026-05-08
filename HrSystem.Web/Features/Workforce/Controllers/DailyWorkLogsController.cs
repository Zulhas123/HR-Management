using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class DailyWorkLogsController(
    ICrudService<DailyWorkLog> logs,
    ICrudService<Employee> employees,
    ICrudService<EmployeeTask> tasks) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await logs.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new DailyWorkLogFormVm();
        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DailyWorkLogFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        await logs.CreateAsync(new DailyWorkLog
        {
            EmployeeId = vm.EmployeeId,
            Date = DateOnly.FromDateTime(vm.Date),
            MinutesWorked = vm.MinutesWorked,
            IsWorkFromHome = vm.IsWorkFromHome,
            EmployeeTaskId = vm.EmployeeTaskId,
            Summary = string.IsNullOrWhiteSpace(vm.Summary) ? null : vm.Summary.Trim()
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await logs.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new DailyWorkLogFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Date = entity.Date.ToDateTime(TimeOnly.MinValue),
            MinutesWorked = entity.MinutesWorked,
            IsWorkFromHome = entity.IsWorkFromHome,
            EmployeeTaskId = entity.EmployeeTaskId,
            Summary = entity.Summary
        };

        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DailyWorkLogFormVm vm, CancellationToken cancellationToken)
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

        var entity = await logs.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.EmployeeId = vm.EmployeeId;
        entity.Date = DateOnly.FromDateTime(vm.Date);
        entity.MinutesWorked = vm.MinutesWorked;
        entity.IsWorkFromHome = vm.IsWorkFromHome;
        entity.EmployeeTaskId = vm.EmployeeTaskId;
        entity.Summary = string.IsNullOrWhiteSpace(vm.Summary) ? null : vm.Summary.Trim();

        await logs.UpdateAsync(entity, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await logs.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await logs.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookupsAsync(DailyWorkLogFormVm vm, CancellationToken cancellationToken)
    {
        var employeeItems = await employees.ListAsync(cancellationToken);
        vm.Employees = new SelectList(employeeItems, nameof(Employee.Id), nameof(Employee.EmployeeCode), vm.EmployeeId);

        var taskItems = await tasks.ListAsync(cancellationToken);
        vm.Tasks = new SelectList(taskItems, nameof(EmployeeTask.Id), nameof(EmployeeTask.Title), vm.EmployeeTaskId);
    }
}
