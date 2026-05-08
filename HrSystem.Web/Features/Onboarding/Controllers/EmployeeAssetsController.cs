using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class EmployeeAssetsController(
    ICrudService<EmployeeAssetAssignment> assets,
    ICrudService<Employee> employees) : Controller
{
    public async Task<IActionResult> Index(int? employeeId, CancellationToken cancellationToken)
    {
        var items = await assets.ListAsync(cancellationToken);
        if (employeeId.HasValue)
        {
            items = items.Where(x => x.EmployeeId == employeeId.Value).ToList();
        }

        ViewBag.EmployeeId = employeeId;
        return View(items);
    }

    public async Task<IActionResult> Create(int? employeeId, CancellationToken cancellationToken)
    {
        var vm = new EmployeeAssetAssignmentVm { EmployeeId = employeeId ?? 0 };
        await PopulateEmployeesAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeAssetAssignmentVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEmployeesAsync(vm, cancellationToken);
            return View(vm);
        }

        await assets.CreateAsync(new EmployeeAssetAssignment
        {
            EmployeeId = vm.EmployeeId,
            AssetName = vm.AssetName,
            AssetTag = vm.AssetTag,
            SerialNumber = vm.SerialNumber,
            AssignedBy = vm.AssignedBy,
            ConditionOnAssign = vm.ConditionOnAssign,
            Status = AssetAssignmentStatus.Assigned
        }, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await assets.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new EmployeeAssetAssignmentVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            AssetName = entity.AssetName,
            AssetTag = entity.AssetTag,
            SerialNumber = entity.SerialNumber,
            AssignedBy = entity.AssignedBy,
            ConditionOnAssign = entity.ConditionOnAssign,
            Status = entity.Status,
            ReturnedTo = entity.ReturnedTo,
            ConditionOnReturn = entity.ConditionOnReturn
        };

        await PopulateEmployeesAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeAssetAssignmentVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateEmployeesAsync(vm, cancellationToken);
            return View(vm);
        }

        var entity = await assets.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.EmployeeId = vm.EmployeeId;
        entity.AssetName = vm.AssetName;
        entity.AssetTag = vm.AssetTag;
        entity.SerialNumber = vm.SerialNumber;
        entity.AssignedBy = vm.AssignedBy;
        entity.ConditionOnAssign = vm.ConditionOnAssign;
        entity.Status = vm.Status;
        entity.ReturnedTo = vm.ReturnedTo;
        entity.ConditionOnReturn = vm.ConditionOnReturn;
        await assets.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkReturned(int id, string? returnedTo, string? conditionOnReturn, CancellationToken cancellationToken)
    {
        var entity = await assets.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Status = AssetAssignmentStatus.Returned;
        entity.ReturnedAtUtc = DateTimeOffset.UtcNow;
        entity.ReturnedTo = returnedTo ?? (User?.Identity?.Name ?? "system");
        entity.ConditionOnReturn = conditionOnReturn;
        await assets.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = entity.EmployeeId });
    }

    private async Task PopulateEmployeesAsync(EmployeeAssetAssignmentVm vm, CancellationToken cancellationToken)
    {
        vm.Employees = (await employees.ListAsync(cancellationToken))
            .Select(e => new SelectListItem($"{e.EmployeeCode} - {e.FirstName} {e.LastName}", e.Id.ToString(), e.Id == vm.EmployeeId))
            .ToList();
    }
}
