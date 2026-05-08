using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class JobApplicationsController(
    ICrudService<JobApplication> applications,
    ICrudService<JobPosting> jobPostings,
    ICrudService<Candidate> candidates) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await applications.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new JobApplicationFormVm();
        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobApplicationFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        await applications.CreateAsync(new JobApplication
        {
            JobPostingId = vm.JobPostingId,
            CandidateId = vm.CandidateId,
            Stage = vm.Stage,
            Notes = vm.Notes
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await applications.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new JobApplicationFormVm
        {
            Id = entity.Id,
            JobPostingId = entity.JobPostingId,
            CandidateId = entity.CandidateId,
            Stage = entity.Stage,
            Notes = entity.Notes
        };

        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, JobApplicationFormVm vm, CancellationToken cancellationToken)
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

        var entity = await applications.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.JobPostingId = vm.JobPostingId;
        entity.CandidateId = vm.CandidateId;
        entity.Stage = vm.Stage;
        entity.Notes = vm.Notes;
        await applications.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await applications.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await applications.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookupsAsync(JobApplicationFormVm vm, CancellationToken cancellationToken)
    {
        vm.JobPostings = (await jobPostings.ListAsync(cancellationToken))
            .Select(j => new SelectListItem(j.Title, j.Id.ToString()))
            .ToList();

        vm.Candidates = (await candidates.ListAsync(cancellationToken))
            .Select(c => new SelectListItem(c.FullName, c.Id.ToString()))
            .ToList();
    }
}

