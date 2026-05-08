using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class EmployeeFamilyMembersController(
    ICrudService<Employee> employees,
    ICrudService<EmployeeFamilyMember> family) : Controller
{
    public async Task<IActionResult> Index(int employeeId, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var items = (await family.ListAsync(cancellationToken))
            .Where(m => m.EmployeeId == employeeId)
            .ToList();

        ViewData["Employee"] = employee;
        return View(items);
    }

    public IActionResult Create(int employeeId) => View(new EmployeeFamilyMemberFormVm { EmployeeId = employeeId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFamilyMemberFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await family.CreateAsync(new EmployeeFamilyMember
        {
            EmployeeId = vm.EmployeeId,
            Name = vm.Name,
            Relationship = vm.Relationship,
            DateOfBirth = vm.DateOfBirth is null ? null : DateOnly.FromDateTime(vm.DateOfBirth.Value),
            Phone = vm.Phone,
            Notes = vm.Notes
        }, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await family.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new EmployeeFamilyMemberFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Name = entity.Name,
            Relationship = entity.Relationship,
            DateOfBirth = entity.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
            Phone = entity.Phone,
            Notes = entity.Notes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeFamilyMemberFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await family.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = vm.Name;
        entity.Relationship = vm.Relationship;
        entity.DateOfBirth = vm.DateOfBirth is null ? null : DateOnly.FromDateTime(vm.DateOfBirth.Value);
        entity.Phone = vm.Phone;
        entity.Notes = vm.Notes;
        await family.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await family.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var entity = await family.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        await family.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index), new { employeeId = entity.EmployeeId });
    }
}
