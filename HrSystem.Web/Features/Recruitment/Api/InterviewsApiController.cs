using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/interviews")]
[Authorize]
public sealed class InterviewsApiController(ICrudService<Interview> interviews) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Interview>>> List(CancellationToken cancellationToken) =>
        Ok(await interviews.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Interview>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await interviews.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Interview>> Create(Interview interview, CancellationToken cancellationToken)
    {
        var created = await interviews.CreateAsync(interview, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Interview interview, CancellationToken cancellationToken)
    {
        if (id != interview.Id)
        {
            return BadRequest();
        }

        await interviews.UpdateAsync(interview, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await interviews.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
