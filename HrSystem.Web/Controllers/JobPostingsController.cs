using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class JobPostingsController(ICrudService<JobPosting> jobPostings) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await jobPostings.ListAsync(cancellationToken);
        return View(items);
    }

    public IActionResult Create() => View(new JobPostingFormVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobPostingFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await jobPostings.CreateAsync(new JobPosting
        {
            Title = vm.Title,
            Department = vm.Department,
            Location = vm.Location,
            EmploymentType = vm.EmploymentType,
            Description = vm.Description,
            IsOpen = vm.IsOpen
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await jobPostings.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new JobPostingFormVm
        {
            Id = entity.Id,
            Title = entity.Title,
            Department = entity.Department,
            Location = entity.Location,
            EmploymentType = entity.EmploymentType,
            Description = entity.Description,
            IsOpen = entity.IsOpen
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, JobPostingFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await jobPostings.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Title = vm.Title;
        entity.Department = vm.Department;
        entity.Location = vm.Location;
        entity.EmploymentType = vm.EmploymentType;
        entity.Description = vm.Description;
        entity.IsOpen = vm.IsOpen;
        await jobPostings.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await jobPostings.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await jobPostings.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}

