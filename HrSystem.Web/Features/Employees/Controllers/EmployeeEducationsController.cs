using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class EmployeeEducationsController(
    ICrudService<Employee> employees,
    ICrudService<EmployeeEducation> educations) : Controller
{
    public async Task<IActionResult> Index(int employeeId, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var items = (await educations.ListAsync(cancellationToken))
            .Where(e => e.EmployeeId == employeeId)
            .ToList();

        ViewData["Employee"] = employee;
        return View(items);
    }

    public IActionResult Create(int employeeId) => View(new EmployeeEducationFormVm { EmployeeId = employeeId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeEducationFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await educations.CreateAsync(new EmployeeEducation
        {
            EmployeeId = vm.EmployeeId,
            Degree = vm.Degree,
            Institution = vm.Institution,
            PassingYear = vm.PassingYear,
            Result = vm.Result
        }, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await educations.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new EmployeeEducationFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Degree = entity.Degree,
            Institution = entity.Institution,
            PassingYear = entity.PassingYear,
            Result = entity.Result
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeEducationFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await educations.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Degree = vm.Degree;
        entity.Institution = vm.Institution;
        entity.PassingYear = vm.PassingYear;
        entity.Result = vm.Result;
        await educations.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await educations.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var entity = await educations.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        await educations.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index), new { employeeId = entity.EmployeeId });
    }
}
