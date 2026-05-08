using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class EmployeeDocumentsController(
    ICrudService<Employee> employees,
    ICrudService<EmployeeDocument> documents) : Controller
{
    public async Task<IActionResult> Index(int employeeId, CancellationToken cancellationToken)
    {
        var employee = await employees.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var docs = (await documents.ListAsync(cancellationToken))
            .Where(d => d.EmployeeId == employeeId)
            .ToList();

        ViewData["Employee"] = employee;
        return View(docs);
    }

    public IActionResult Upload(int employeeId) => View(new EmployeeDocumentFormVm { EmployeeId = employeeId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(EmployeeDocumentFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || vm.File is null || vm.File.Length == 0)
        {
            return View(vm);
        }

        var employee = await employees.GetByIdAsync(vm.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "employees", vm.EmployeeId.ToString(), "documents");
        Directory.CreateDirectory(uploadsRoot);

        var ext = Path.GetExtension(vm.File.FileName);
        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploadsRoot, storedFileName);

        await using (var fs = new FileStream(fullPath, FileMode.Create))
        {
            await vm.File.CopyToAsync(fs, cancellationToken);
        }

        await documents.CreateAsync(new EmployeeDocument
        {
            EmployeeId = vm.EmployeeId,
            DocumentName = vm.DocumentName,
            DocumentType = vm.DocumentType,
            StoredPath = $"/uploads/employees/{vm.EmployeeId}/documents/{storedFileName}",
            OriginalFileName = vm.File.FileName,
            UploadedAtUtc = DateTime.UtcNow
        }, cancellationToken);

        return RedirectToAction(nameof(Index), new { employeeId = vm.EmployeeId });
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var doc = await documents.GetByIdAsync(id, cancellationToken);
        return doc is null ? NotFound() : View(doc);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var doc = await documents.GetByIdAsync(id, cancellationToken);
        if (doc is null)
        {
            return NotFound();
        }

        await documents.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index), new { employeeId = doc.EmployeeId });
    }
}

