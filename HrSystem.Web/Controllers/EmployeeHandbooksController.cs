using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class EmployeeHandbooksController(
    ICrudService<EmployeeHandbook> handbooks,
    ICrudService<EmployeeHandbookAcknowledgement> acknowledgements,
    ICrudService<Employee> employees) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await handbooks.ListAsync(cancellationToken);
        return View(items);
    }

    public IActionResult Create() => View(new EmployeeHandbookFormVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeHandbookFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        if (vm.File is null || vm.File.Length <= 0)
        {
            ModelState.AddModelError(nameof(vm.File), "Handbook file is required.");
            return View(vm);
        }

        var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "handbooks");
        Directory.CreateDirectory(uploadsRoot);

        var ext = Path.GetExtension(vm.File.FileName);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsRoot, fileName);

        await using (var fs = new FileStream(fullPath, FileMode.Create))
        {
            await vm.File.CopyToAsync(fs, cancellationToken);
        }

        await handbooks.CreateAsync(new EmployeeHandbook
        {
            Title = vm.Title,
            FilePath = $"/uploads/handbooks/{fileName}"
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Acknowledge(int handbookId, CancellationToken cancellationToken)
    {
        var handbook = await handbooks.GetByIdAsync(handbookId, cancellationToken);
        if (handbook is null)
        {
            return NotFound();
        }

        var vm = new EmployeeHandbookAcknowledgeVm { EmployeeHandbookId = handbookId };
        await PopulateEmployeesAsync(vm, cancellationToken);
        ViewBag.Handbook = handbook;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Acknowledge(EmployeeHandbookAcknowledgeVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEmployeesAsync(vm, cancellationToken);
            return View(vm);
        }

        await acknowledgements.CreateAsync(new EmployeeHandbookAcknowledgement
        {
            EmployeeId = vm.EmployeeId,
            EmployeeHandbookId = vm.EmployeeHandbookId
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateEmployeesAsync(EmployeeHandbookAcknowledgeVm vm, CancellationToken cancellationToken)
    {
        vm.Employees = (await employees.ListAsync(cancellationToken))
            .Select(e => new SelectListItem($"{e.EmployeeCode} - {e.FirstName} {e.LastName}", e.Id.ToString(), e.Id == vm.EmployeeId))
            .ToList();
    }
}

