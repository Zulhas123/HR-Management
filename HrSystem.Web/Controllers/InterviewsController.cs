using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class InterviewsController(
    ICrudService<Interview> interviews,
    ICrudService<JobApplication> applications) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await interviews.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new InterviewFormVm();
        await PopulateAppsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InterviewFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAppsAsync(vm, cancellationToken);
            return View(vm);
        }

        await interviews.CreateAsync(new Interview
        {
            JobApplicationId = vm.JobApplicationId,
            ScheduledAtUtc = DateTime.SpecifyKind(vm.ScheduledAtUtc, DateTimeKind.Utc),
            Interviewer = vm.Interviewer,
            Mode = vm.Mode,
            Feedback = vm.Feedback,
            Result = vm.Result
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await interviews.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new InterviewFormVm
        {
            Id = entity.Id,
            JobApplicationId = entity.JobApplicationId,
            ScheduledAtUtc = entity.ScheduledAtUtc,
            Interviewer = entity.Interviewer,
            Mode = entity.Mode,
            Feedback = entity.Feedback,
            Result = entity.Result
        };

        await PopulateAppsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InterviewFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateAppsAsync(vm, cancellationToken);
            return View(vm);
        }

        var entity = await interviews.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.JobApplicationId = vm.JobApplicationId;
        entity.ScheduledAtUtc = DateTime.SpecifyKind(vm.ScheduledAtUtc, DateTimeKind.Utc);
        entity.Interviewer = vm.Interviewer;
        entity.Mode = vm.Mode;
        entity.Feedback = vm.Feedback;
        entity.Result = vm.Result;
        await interviews.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await interviews.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await interviews.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAppsAsync(InterviewFormVm vm, CancellationToken cancellationToken)
    {
        vm.Applications = (await applications.ListAsync(cancellationToken))
            .Select(a => new SelectListItem(
                $"{a.JobPosting?.Title} - {a.Candidate?.FullName} ({a.Stage})",
                a.Id.ToString()))
            .ToList();
    }
}

