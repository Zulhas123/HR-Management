using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/shifts")]
[Authorize]
public sealed class ShiftsApiController(ICrudService<Shift> shifts) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Shift>>> List(CancellationToken cancellationToken) =>
        Ok(await shifts.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Shift>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await shifts.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Shift>> Create(Shift shift, CancellationToken cancellationToken)
    {
        var created = await shifts.CreateAsync(shift, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Shift shift, CancellationToken cancellationToken)
    {
        if (id != shift.Id)
        {
            return BadRequest();
        }

        await shifts.UpdateAsync(shift, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await shifts.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

