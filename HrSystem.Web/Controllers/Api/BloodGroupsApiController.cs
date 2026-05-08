using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/blood-groups")]
[Authorize]
public sealed class BloodGroupsApiController(ICrudService<BloodGroup> bloodGroups) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BloodGroup>>> List(CancellationToken cancellationToken) =>
        Ok(await bloodGroups.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BloodGroup>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await bloodGroups.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<BloodGroup>> Create(BloodGroup bloodGroup, CancellationToken cancellationToken)
    {
        var created = await bloodGroups.CreateAsync(bloodGroup, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BloodGroup bloodGroup, CancellationToken cancellationToken)
    {
        if (id != bloodGroup.Id) return BadRequest();
        await bloodGroups.UpdateAsync(bloodGroup, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await bloodGroups.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

