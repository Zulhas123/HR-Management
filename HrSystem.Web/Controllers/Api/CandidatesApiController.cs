using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/candidates")]
[Authorize]
public sealed class CandidatesApiController(ICrudService<Candidate> candidates) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Candidate>>> List(CancellationToken cancellationToken) =>
        Ok(await candidates.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Candidate>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await candidates.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Candidate>> Create(Candidate candidate, CancellationToken cancellationToken)
    {
        var created = await candidates.CreateAsync(candidate, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Candidate candidate, CancellationToken cancellationToken)
    {
        if (id != candidate.Id)
        {
            return BadRequest();
        }

        await candidates.UpdateAsync(candidate, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await candidates.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

