using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/designations")]
[Authorize]
public sealed class DesignationsApiController(ICrudService<Designation> designations) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Designation>>> List(CancellationToken cancellationToken) =>
        Ok(await designations.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Designation>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await designations.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Designation>> Create(Designation designation, CancellationToken cancellationToken)
    {
        var created = await designations.CreateAsync(designation, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Designation designation, CancellationToken cancellationToken)
    {
        if (id != designation.Id)
        {
            return BadRequest();
        }

        await designations.UpdateAsync(designation, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await designations.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
