using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class OffboardingsController(
    ICrudService<EmployeeOffboarding> offboardings,
    ICrudService<Employee> employees,
    ICrudService<EmployeeAssetAssignment> assets,
    ICrudService<ExitInterview> interviews,
    ICrudService<OffboardingClearanceItem> clearance,
    ICrudService<FinalSettlement> settlements) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await offboardings.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new OffboardingFormVm();
        await PopulateEmployeesAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OffboardingFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEmployeesAsync(vm, cancellationToken);
            return View(vm);
        }

        var created = await offboardings.CreateAsync(new EmployeeOffboarding
        {
            EmployeeId = vm.EmployeeId,
            LastWorkingDay = DateOnly.FromDateTime(vm.LastWorkingDay),
            Reason = vm.Reason,
            Status = vm.Status
        }, cancellationToken);

        // Seed common clearance departments (MVP)
        await clearance.CreateAsync(new OffboardingClearanceItem { EmployeeOffboardingId = created.Id, DepartmentName = "HR" }, cancellationToken);
        await clearance.CreateAsync(new OffboardingClearanceItem { EmployeeOffboardingId = created.Id, DepartmentName = "Accounts" }, cancellationToken);
        await clearance.CreateAsync(new OffboardingClearanceItem { EmployeeOffboardingId = created.Id, DepartmentName = "IT" }, cancellationToken);
        await clearance.CreateAsync(new OffboardingClearanceItem { EmployeeOffboardingId = created.Id, DepartmentName = "Admin" }, cancellationToken);

        return RedirectToAction(nameof(Details), new { id = created.Id });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var offboarding = await offboardings.GetByIdAsync(id, cancellationToken);
        if (offboarding is null)
        {
            return NotFound();
        }

        var employee = await employees.GetByIdAsync(offboarding.EmployeeId, cancellationToken);
        var exitInterview = (await interviews.ListAsync(cancellationToken)).FirstOrDefault(x => x.EmployeeOffboardingId == id);
        var clearanceItems = (await clearance.ListAsync(cancellationToken)).Where(x => x.EmployeeOffboardingId == id).ToList();
        var settlement = (await settlements.ListAsync(cancellationToken)).FirstOrDefault(x => x.EmployeeOffboardingId == id);
        var employeeAssets = (await assets.ListAsync(cancellationToken)).Where(x => x.EmployeeId == offboarding.EmployeeId).ToList();
        var outstandingAssets = employeeAssets.Where(a => a.Status == AssetAssignmentStatus.Assigned).ToList();

        ViewBag.Employee = employee;
        ViewBag.ExitInterview = exitInterview;
        ViewBag.ClearanceItems = clearanceItems.OrderBy(x => x.DepartmentName).ToList();
        ViewBag.Settlement = settlement;
        ViewBag.OutstandingAssets = outstandingAssets;

        return View(offboarding);
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await offboardings.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new OffboardingFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            LastWorkingDay = entity.LastWorkingDay.ToDateTime(TimeOnly.MinValue),
            Reason = entity.Reason,
            Status = entity.Status
        };

        await PopulateEmployeesAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OffboardingFormVm vm, CancellationToken cancellationToken)
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

        var entity = await offboardings.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (vm.Status == OffboardingStatus.Completed)
        {
            var clearanceItems = (await clearance.ListAsync(cancellationToken)).Where(x => x.EmployeeOffboardingId == id).ToList();
            if (clearanceItems.Any() && clearanceItems.Any(x => x.Decision != ClearanceDecision.Cleared))
            {
                ModelState.AddModelError(nameof(vm.Status), "Cannot complete offboarding until all clearance items are Cleared.");
            }

            var outstandingAssets = (await assets.ListAsync(cancellationToken))
                .Where(x => x.EmployeeId == vm.EmployeeId && x.Status == AssetAssignmentStatus.Assigned)
                .ToList();
            if (outstandingAssets.Any())
            {
                ModelState.AddModelError(nameof(vm.Status), "Cannot complete offboarding until all assigned assets are returned.");
            }

            var settlement = (await settlements.ListAsync(cancellationToken)).FirstOrDefault(x => x.EmployeeOffboardingId == id);
            if (settlement is null)
            {
                ModelState.AddModelError(nameof(vm.Status), "Cannot complete offboarding until final settlement is prepared.");
            }
        }

        if (!ModelState.IsValid)
        {
            await PopulateEmployeesAsync(vm, cancellationToken);
            return View(vm);
        }

        entity.EmployeeId = vm.EmployeeId;
        entity.LastWorkingDay = DateOnly.FromDateTime(vm.LastWorkingDay);
        entity.Reason = vm.Reason;
        entity.Status = vm.Status;
        if (entity.Status == OffboardingStatus.Completed && entity.CompletedAtUtc is null)
        {
            entity.CompletedAtUtc = DateTimeOffset.UtcNow;
        }

        await offboardings.UpdateAsync(entity, cancellationToken);
        return RedirectToAction(nameof(Details), new { id });
    }

    // Exit interview
    public async Task<IActionResult> EditExitInterview(int offboardingId, CancellationToken cancellationToken)
    {
        var existing = (await interviews.ListAsync(cancellationToken)).FirstOrDefault(x => x.EmployeeOffboardingId == offboardingId);
        var vm = new ExitInterviewVm
        {
            Id = existing?.Id ?? 0,
            EmployeeOffboardingId = offboardingId,
            InterviewDate = (existing?.InterviewDate ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue),
            Interviewer = existing?.Interviewer,
            Notes = existing?.Notes
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditExitInterview(ExitInterviewVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        ExitInterview entity;
        if (vm.Id == 0)
        {
            entity = new ExitInterview { EmployeeOffboardingId = vm.EmployeeOffboardingId };
        }
        else
        {
            entity = await interviews.GetByIdAsync(vm.Id, cancellationToken) ?? new ExitInterview { EmployeeOffboardingId = vm.EmployeeOffboardingId };
        }

        entity.InterviewDate = DateOnly.FromDateTime(vm.InterviewDate);
        entity.Interviewer = vm.Interviewer;
        entity.Notes = vm.Notes;

        if (entity.Id == 0)
        {
            await interviews.CreateAsync(entity, cancellationToken);
        }
        else
        {
            await interviews.UpdateAsync(entity, cancellationToken);
        }

        return RedirectToAction(nameof(Details), new { id = vm.EmployeeOffboardingId });
    }

    // Clearance workflow
    public IActionResult AddClearanceItem(int offboardingId) =>
        View(new ClearanceItemVm { EmployeeOffboardingId = offboardingId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddClearanceItem(ClearanceItemVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await clearance.CreateAsync(new OffboardingClearanceItem
        {
            EmployeeOffboardingId = vm.EmployeeOffboardingId,
            DepartmentName = vm.DepartmentName,
            Decision = vm.Decision,
            Note = vm.Note,
            DecidedAtUtc = vm.Decision == ClearanceDecision.Pending ? null : DateTimeOffset.UtcNow,
            DecidedBy = vm.Decision == ClearanceDecision.Pending ? null : (User?.Identity?.Name ?? "system")
        }, cancellationToken);

        return RedirectToAction(nameof(Details), new { id = vm.EmployeeOffboardingId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetClearanceDecision(int id, int offboardingId, ClearanceDecision decision, string? note, CancellationToken cancellationToken)
    {
        var item = await clearance.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        item.Decision = decision;
        item.Note = note;
        item.DecidedAtUtc = decision == ClearanceDecision.Pending ? null : DateTimeOffset.UtcNow;
        item.DecidedBy = decision == ClearanceDecision.Pending ? null : (User?.Identity?.Name ?? "system");
        await clearance.UpdateAsync(item, cancellationToken);

        return RedirectToAction(nameof(Details), new { id = offboardingId });
    }

    // Final settlement
    public async Task<IActionResult> EditSettlement(int offboardingId, CancellationToken cancellationToken)
    {
        var existing = (await settlements.ListAsync(cancellationToken)).FirstOrDefault(x => x.EmployeeOffboardingId == offboardingId);
        var vm = new FinalSettlementVm
        {
            Id = existing?.Id ?? 0,
            EmployeeOffboardingId = offboardingId,
            TotalPayable = existing?.TotalPayable ?? 0,
            TotalDeductions = existing?.TotalDeductions ?? 0,
            NetPayable = existing?.NetPayable ?? 0,
            PreparedBy = existing?.PreparedBy,
            Notes = existing?.Notes
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSettlement(FinalSettlementVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        FinalSettlement entity;
        if (vm.Id == 0)
        {
            entity = new FinalSettlement { EmployeeOffboardingId = vm.EmployeeOffboardingId };
        }
        else
        {
            entity = await settlements.GetByIdAsync(vm.Id, cancellationToken) ?? new FinalSettlement { EmployeeOffboardingId = vm.EmployeeOffboardingId };
        }

        entity.TotalPayable = vm.TotalPayable;
        entity.TotalDeductions = vm.TotalDeductions;
        entity.NetPayable = vm.TotalPayable - vm.TotalDeductions;
        entity.PreparedBy = vm.PreparedBy ?? (User?.Identity?.Name ?? "system");
        entity.Notes = vm.Notes;

        if (entity.Id == 0)
        {
            await settlements.CreateAsync(entity, cancellationToken);
        }
        else
        {
            await settlements.UpdateAsync(entity, cancellationToken);
        }

        return RedirectToAction(nameof(Details), new { id = vm.EmployeeOffboardingId });
    }

    public async Task<IActionResult> ExperienceCertificate(int offboardingId, CancellationToken cancellationToken)
    {
        var offboarding = await offboardings.GetByIdAsync(offboardingId, cancellationToken);
        if (offboarding is null)
        {
            return NotFound();
        }

        var employee = await employees.GetByIdAsync(offboarding.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        ViewBag.Employee = employee;
        return View(offboarding);
    }

    private async Task PopulateEmployeesAsync(OffboardingFormVm vm, CancellationToken cancellationToken)
    {
        vm.Employees = (await employees.ListAsync(cancellationToken))
            .Select(e => new SelectListItem($"{e.EmployeeCode} - {e.FirstName} {e.LastName}", e.Id.ToString(), e.Id == vm.EmployeeId))
            .ToList();
    }
}
