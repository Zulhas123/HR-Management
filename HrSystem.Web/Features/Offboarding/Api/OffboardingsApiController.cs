using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/offboardings")]
[Authorize]
public sealed class OffboardingsApiController(ICrudService<EmployeeOffboarding> offboardings) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeOffboarding>>> List(CancellationToken cancellationToken) =>
        Ok(await offboardings.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeOffboarding>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await offboardings.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeOffboarding>> Create(EmployeeOffboarding offboarding, CancellationToken cancellationToken)
    {
        var created = await offboardings.CreateAsync(offboarding, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeOffboarding offboarding, CancellationToken cancellationToken)
    {
        if (id != offboarding.Id)
        {
            return BadRequest();
        }

        await offboardings.UpdateAsync(offboarding, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await offboardings.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
