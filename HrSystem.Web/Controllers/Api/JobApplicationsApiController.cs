using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/job-applications")]
[Authorize]
public sealed class JobApplicationsApiController(ICrudService<JobApplication> applications) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<JobApplication>>> List(CancellationToken cancellationToken) =>
        Ok(await applications.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JobApplication>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await applications.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplication>> Create(JobApplication application, CancellationToken cancellationToken)
    {
        var created = await applications.CreateAsync(application, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, JobApplication application, CancellationToken cancellationToken)
    {
        if (id != application.Id)
        {
            return BadRequest();
        }

        await applications.UpdateAsync(application, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await applications.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

