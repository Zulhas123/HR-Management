using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class EmployeeExperiencesController(
    ICrudService<Employee> employees,
    ICrudService<EmployeeExperience> experiences) : Controller
{
    public async Task<IActionResult> Index(int employeeId, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var items = (await experiences.ListAsync(cancellationToken))
            .Where(e => e.EmployeeId == employeeId)
            .ToList();

        ViewData["Employee"] = employee;
        return View(items);
    }

    public IActionResult Create(int employeeId) => View(new EmployeeExperienceFormVm { EmployeeId = employeeId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeExperienceFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await experiences.CreateAsync(new EmployeeExperience
        {
            EmployeeId = vm.EmployeeId,
            CompanyName = vm.CompanyName,
            Designation = vm.Designation,
            StartDate = vm.StartDate is null ? null : DateOnly.FromDateTime(vm.StartDate.Value),
            EndDate = vm.EndDate is null ? null : DateOnly.FromDateTime(vm.EndDate.Value),
            Notes = vm.Notes
        }, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await experiences.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new EmployeeExperienceFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            CompanyName = entity.CompanyName,
            Designation = entity.Designation,
            StartDate = entity.StartDate?.ToDateTime(TimeOnly.MinValue),
            EndDate = entity.EndDate?.ToDateTime(TimeOnly.MinValue),
            Notes = entity.Notes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeExperienceFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await experiences.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.CompanyName = vm.CompanyName;
        entity.Designation = vm.Designation;
        entity.StartDate = vm.StartDate is null ? null : DateOnly.FromDateTime(vm.StartDate.Value);
        entity.EndDate = vm.EndDate is null ? null : DateOnly.FromDateTime(vm.EndDate.Value);
        entity.Notes = vm.Notes;
        await experiences.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await experiences.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var entity = await experiences.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        await experiences.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index), new { employeeId = entity.EmployeeId });
    }
}

