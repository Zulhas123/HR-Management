using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class OnboardingsController(
    ICrudService<EmployeeOnboarding> onboardings,
    ICrudService<Employee> employees,
    ICrudService<EmployeeJoiningForm> joiningForms,
    ICrudService<Department> departments,
    ICrudService<Designation> designations,
    ICrudService<EmploymentType> employmentTypes,
    ICrudService<OnboardingDocumentChecklistItem> documents,
    ICrudService<OnboardingOrientationItem> orientation) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await onboardings.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new OnboardingFormVm();
        await PopulateEmployeesAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OnboardingFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEmployeesAsync(vm, cancellationToken);
            return View(vm);
        }

        var created = await onboardings.CreateAsync(new EmployeeOnboarding
        {
            EmployeeId = vm.EmployeeId,
            Status = vm.Status
        }, cancellationToken);

        // Seed common checklists (MVP)
        await documents.CreateAsync(new OnboardingDocumentChecklistItem { EmployeeOnboardingId = created.Id, Name = "NID Copy", IsRequired = true }, cancellationToken);
        await documents.CreateAsync(new OnboardingDocumentChecklistItem { EmployeeOnboardingId = created.Id, Name = "Photo", IsRequired = true }, cancellationToken);
        await documents.CreateAsync(new OnboardingDocumentChecklistItem { EmployeeOnboardingId = created.Id, Name = "Educational Certificates", IsRequired = true }, cancellationToken);

        await orientation.CreateAsync(new OnboardingOrientationItem { EmployeeOnboardingId = created.Id, Title = "Company introduction", IsCompleted = false }, cancellationToken);
        await orientation.CreateAsync(new OnboardingOrientationItem { EmployeeOnboardingId = created.Id, Title = "HR policy briefing", IsCompleted = false }, cancellationToken);
        await orientation.CreateAsync(new OnboardingOrientationItem { EmployeeOnboardingId = created.Id, Title = "IT access setup", IsCompleted = false }, cancellationToken);

        return RedirectToAction(nameof(Details), new { id = created.Id });
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var onboarding = await onboardings.GetByIdAsync(id, cancellationToken);
        if (onboarding is null)
        {
            return NotFound();
        }

        var employee = await employees.GetByIdAsync(onboarding.EmployeeId, cancellationToken);
        var joiningForm = (await joiningForms.ListAsync(cancellationToken)).FirstOrDefault(x => x.EmployeeOnboardingId == id);
        var checklist = (await documents.ListAsync(cancellationToken)).Where(x => x.EmployeeOnboardingId == id).ToList();
        var orientationItems = (await orientation.ListAsync(cancellationToken)).Where(x => x.EmployeeOnboardingId == id).ToList();

        ViewBag.Employee = employee;
        ViewBag.JoiningForm = joiningForm;
        ViewBag.DocumentChecklist = checklist.OrderBy(x => x.Id).ToList();
        ViewBag.OrientationChecklist = orientationItems.OrderBy(x => x.Id).ToList();

        return View(onboarding);
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await onboardings.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new OnboardingFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Status = entity.Status
        };
        await PopulateEmployeesAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OnboardingFormVm vm, CancellationToken cancellationToken)
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

        var entity = await onboardings.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.EmployeeId = vm.EmployeeId;
        entity.Status = vm.Status;
        await onboardings.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Details), new { id });
    }

    // Joining form (digital joining form + department assignment)
    public async Task<IActionResult> EditJoiningForm(int onboardingId, CancellationToken cancellationToken)
    {
        var onboarding = await onboardings.GetByIdAsync(onboardingId, cancellationToken);
        if (onboarding is null)
        {
            return NotFound();
        }

        var existing = (await joiningForms.ListAsync(cancellationToken)).FirstOrDefault(x => x.EmployeeOnboardingId == onboardingId);
        var vm = new JoiningFormVm
        {
            Id = existing?.Id ?? 0,
            EmployeeOnboardingId = onboardingId,
            JoinDate = (existing?.JoinDate ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue),
            DepartmentId = existing?.DepartmentId ?? 0,
            DesignationId = existing?.DesignationId ?? 0,
            EmploymentTypeId = existing?.EmploymentTypeId ?? 0,
            Notes = existing?.Notes
        };

        await PopulateJoiningLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditJoiningForm(JoiningFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateJoiningLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        EmployeeJoiningForm entity;
        if (vm.Id == 0)
        {
            entity = new EmployeeJoiningForm { EmployeeOnboardingId = vm.EmployeeOnboardingId };
        }
        else
        {
            entity = await joiningForms.GetByIdAsync(vm.Id, cancellationToken) ?? new EmployeeJoiningForm { EmployeeOnboardingId = vm.EmployeeOnboardingId };
        }

        entity.JoinDate = DateOnly.FromDateTime(vm.JoinDate);
        entity.DepartmentId = vm.DepartmentId;
        entity.DesignationId = vm.DesignationId;
        entity.EmploymentTypeId = vm.EmploymentTypeId;
        entity.Notes = vm.Notes;

        if (entity.Id == 0)
        {
            entity = await joiningForms.CreateAsync(entity, cancellationToken);
        }
        else
        {
            await joiningForms.UpdateAsync(entity, cancellationToken);
        }

        // Apply department assignment etc. to Employee (MVP)
        var onboarding = await onboardings.GetByIdAsync(vm.EmployeeOnboardingId, cancellationToken);
        if (onboarding is not null)
        {
            var employee = await employees.GetByIdAsync(onboarding.EmployeeId, cancellationToken);
            if (employee is not null)
            {
                employee.JoinDate = entity.JoinDate;
                employee.DepartmentId = entity.DepartmentId;
                employee.DesignationId = entity.DesignationId;
                employee.EmploymentTypeId = entity.EmploymentTypeId;
                await employees.UpdateAsync(employee, cancellationToken);
            }
        }

        return RedirectToAction(nameof(Details), new { id = vm.EmployeeOnboardingId });
    }

    // Document checklist
    public IActionResult AddDocumentItem(int onboardingId) =>
        View(new OnboardingDocumentChecklistItemVm { EmployeeOnboardingId = onboardingId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddDocumentItem(OnboardingDocumentChecklistItemVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await documents.CreateAsync(new OnboardingDocumentChecklistItem
        {
            EmployeeOnboardingId = vm.EmployeeOnboardingId,
            Name = vm.Name,
            IsRequired = vm.IsRequired,
            IsProvided = vm.IsProvided,
            Notes = vm.Notes
        }, cancellationToken);

        return RedirectToAction(nameof(Details), new { id = vm.EmployeeOnboardingId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleDocumentProvided(int id, int onboardingId, CancellationToken cancellationToken)
    {
        var item = await documents.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        item.IsProvided = !item.IsProvided;
        await documents.UpdateAsync(item, cancellationToken);
        return RedirectToAction(nameof(Details), new { id = onboardingId });
    }

    // Orientation checklist
    public IActionResult AddOrientationItem(int onboardingId) =>
        View(new OnboardingOrientationItemVm { EmployeeOnboardingId = onboardingId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrientationItem(OnboardingOrientationItemVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await orientation.CreateAsync(new OnboardingOrientationItem
        {
            EmployeeOnboardingId = vm.EmployeeOnboardingId,
            Title = vm.Title,
            IsCompleted = vm.IsCompleted,
            CompletedAtUtc = vm.IsCompleted ? DateTimeOffset.UtcNow : null,
            CompletedBy = vm.CompletedBy
        }, cancellationToken);

        return RedirectToAction(nameof(Details), new { id = vm.EmployeeOnboardingId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleOrientationCompleted(int id, int onboardingId, CancellationToken cancellationToken)
    {
        var item = await orientation.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        item.IsCompleted = !item.IsCompleted;
        item.CompletedAtUtc = item.IsCompleted ? DateTimeOffset.UtcNow : null;
        item.CompletedBy = item.IsCompleted ? (User?.Identity?.Name ?? "system") : null;
        await orientation.UpdateAsync(item, cancellationToken);
        return RedirectToAction(nameof(Details), new { id = onboardingId });
    }

    private async Task PopulateEmployeesAsync(OnboardingFormVm vm, CancellationToken cancellationToken)
    {
        vm.Employees = (await employees.ListAsync(cancellationToken))
            .Select(e => new SelectListItem($"{e.EmployeeCode} - {e.FirstName} {e.LastName}", e.Id.ToString(), e.Id == vm.EmployeeId))
            .ToList();
    }

    private async Task PopulateJoiningLookupsAsync(JoiningFormVm vm, CancellationToken cancellationToken)
    {
        vm.Departments = (await departments.ListAsync(cancellationToken))
            .Select(d => new SelectListItem(d.Name, d.Id.ToString(), d.Id == vm.DepartmentId))
            .ToList();
        vm.Designations = (await designations.ListAsync(cancellationToken))
            .Select(d => new SelectListItem(d.Name, d.Id.ToString(), d.Id == vm.DesignationId))
            .ToList();
        vm.EmploymentTypes = (await employmentTypes.ListAsync(cancellationToken))
            .Select(e => new SelectListItem(e.Name, e.Id.ToString(), e.Id == vm.EmploymentTypeId))
            .ToList();
    }
}

