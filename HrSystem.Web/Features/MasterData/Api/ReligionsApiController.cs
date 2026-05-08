using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/religions")]
[Authorize]
public sealed class ReligionsApiController(ICrudService<Religion> religions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Religion>>> List(CancellationToken cancellationToken) =>
        Ok(await religions.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Religion>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await religions.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Religion>> Create(Religion religion, CancellationToken cancellationToken)
    {
        var created = await religions.CreateAsync(religion, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Religion religion, CancellationToken cancellationToken)
    {
        if (id != religion.Id) return BadRequest();
        await religions.UpdateAsync(religion, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await religions.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
