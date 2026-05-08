using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/salary-adjustments")]
[Authorize]
public sealed class SalaryAdjustmentsApiController(ICrudService<SalaryAdjustment> adjustments) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SalaryAdjustment>>> List(CancellationToken cancellationToken) =>
        Ok(await adjustments.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SalaryAdjustment>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await adjustments.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<SalaryAdjustment>> Create(SalaryAdjustment adjustment, CancellationToken cancellationToken)
    {
        adjustment.Employee = null;
        var created = await adjustments.CreateAsync(adjustment, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SalaryAdjustment adjustment, CancellationToken cancellationToken)
    {
        if (id != adjustment.Id)
        {
            return BadRequest();
        }

        adjustment.Employee = null;
        await adjustments.UpdateAsync(adjustment, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await adjustments.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
