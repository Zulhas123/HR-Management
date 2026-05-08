using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class EmployeeEmergencyContactsController(
    ICrudService<Employee> employees,
    ICrudService<EmployeeEmergencyContact> contacts) : Controller
{
    public async Task<IActionResult> Index(int employeeId, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var items = (await contacts.ListAsync(cancellationToken))
            .Where(c => c.EmployeeId == employeeId)
            .OrderByDescending(c => c.IsPrimary)
            .ToList();

        ViewData["Employee"] = employee;
        return View(items);
    }

    public IActionResult Create(int employeeId) => View(new EmployeeEmergencyContactFormVm { EmployeeId = employeeId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeEmergencyContactFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await contacts.CreateAsync(new EmployeeEmergencyContact
        {
            EmployeeId = vm.EmployeeId,
            Name = vm.Name,
            Relationship = vm.Relationship,
            Phone = vm.Phone,
            Address = vm.Address,
            IsPrimary = vm.IsPrimary
        }, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await contacts.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new EmployeeEmergencyContactFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Name = entity.Name,
            Relationship = entity.Relationship,
            Phone = entity.Phone,
            Address = entity.Address,
            IsPrimary = entity.IsPrimary
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeEmergencyContactFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await contacts.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Name = vm.Name;
        entity.Relationship = vm.Relationship;
        entity.Phone = vm.Phone;
        entity.Address = vm.Address;
        entity.IsPrimary = vm.IsPrimary;
        await contacts.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await contacts.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var entity = await contacts.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        await contacts.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index), new { employeeId = entity.EmployeeId });
    }
}

