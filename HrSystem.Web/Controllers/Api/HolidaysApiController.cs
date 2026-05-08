using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/holidays")]
[Authorize]
public sealed class HolidaysApiController(ICrudService<Holiday> holidays) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Holiday>>> List(CancellationToken cancellationToken) =>
        Ok(await holidays.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Holiday>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await holidays.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Holiday>> Create(Holiday holiday, CancellationToken cancellationToken)
    {
        var created = await holidays.CreateAsync(holiday, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Holiday holiday, CancellationToken cancellationToken)
    {
        if (id != holiday.Id)
        {
            return BadRequest();
        }

        await holidays.UpdateAsync(holiday, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await holidays.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

