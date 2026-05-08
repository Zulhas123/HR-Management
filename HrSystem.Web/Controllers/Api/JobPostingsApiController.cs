using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/job-postings")]
[Authorize]
public sealed class JobPostingsApiController(ICrudService<JobPosting> jobPostings) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<JobPosting>>> List(CancellationToken cancellationToken) =>
        Ok(await jobPostings.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobPosting>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await jobPostings.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<JobPosting>> Create(JobPosting jobPosting, CancellationToken cancellationToken)
    {
        var created = await jobPostings.CreateAsync(jobPosting, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, JobPosting jobPosting, CancellationToken cancellationToken)
    {
        if (id != jobPosting.Id)
        {
            return BadRequest();
        }

        await jobPostings.UpdateAsync(jobPosting, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await jobPostings.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

