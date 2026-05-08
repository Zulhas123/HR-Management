using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class EmployeesController(
    IEmployeeService employees,
    ICrudService<Department> departments,
    ICrudService<Designation> designations,
    ICrudService<EmploymentType> employmentTypes) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await employees.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var entity = await employees.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new EmployeeFormVm { JoinDate = DateTime.Today };
        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        await employees.CreateAsync(new Employee
        {
            EmployeeCode = "PENDING",
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            Phone = vm.Phone,
            JoinDate = DateOnly.FromDateTime(vm.JoinDate),
            DepartmentId = vm.DepartmentId,
            DesignationId = vm.DesignationId,
            EmploymentTypeId = vm.EmploymentTypeId,
            NidNumber = vm.NidNumber,
            TinNumber = vm.TinNumber,
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await employees.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new EmployeeFormVm
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Phone = entity.Phone,
            JoinDate = entity.JoinDate.ToDateTime(TimeOnly.MinValue),
            DepartmentId = entity.DepartmentId,
            DesignationId = entity.DesignationId,
            EmploymentTypeId = entity.EmploymentTypeId,
            NidNumber = entity.NidNumber,
            TinNumber = entity.TinNumber,
        };

        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeFormVm vm, CancellationToken cancellationToken)
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

        var entity = await employees.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.FirstName = vm.FirstName;
        entity.LastName = vm.LastName;
        entity.Email = vm.Email;
        entity.Phone = vm.Phone;
        entity.JoinDate = DateOnly.FromDateTime(vm.JoinDate);
        entity.DepartmentId = vm.DepartmentId;
        entity.DesignationId = vm.DesignationId;
        entity.EmploymentTypeId = vm.EmploymentTypeId;
        entity.NidNumber = vm.NidNumber;
        entity.TinNumber = vm.TinNumber;

        await employees.UpdateAsync(entity, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await employees.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await employees.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookupsAsync(EmployeeFormVm vm, CancellationToken cancellationToken)
    {
        vm.Departments = (await departments.ListAsync(cancellationToken))
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToList();

        vm.Designations = (await designations.ListAsync(cancellationToken))
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToList();

        vm.EmploymentTypes = (await employmentTypes.ListAsync(cancellationToken))
            .Select(e => new SelectListItem(e.Name, e.Id.ToString()))
            .ToList();
    }
}

