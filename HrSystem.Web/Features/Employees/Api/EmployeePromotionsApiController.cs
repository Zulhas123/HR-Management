using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-promotions")]
[Authorize]
public sealed class EmployeePromotionsApiController(ICrudService<EmployeePromotion> promotions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeePromotion>>> List([FromQuery] int? employeeId, CancellationToken cancellationToken)
    {
        var items = await promotions.ListAsync(cancellationToken);
        if (employeeId is not null)
        {
            items = items.Where(d => d.EmployeeId == employeeId.Value).ToList();
        }

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeePromotion>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await promotions.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeePromotion>> Create(EmployeePromotion promotion, CancellationToken cancellationToken)
    {
        var created = await promotions.CreateAsync(promotion, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeePromotion promotion, CancellationToken cancellationToken)
    {
        if (id != promotion.Id)
        {
            return BadRequest();
        }

        await promotions.UpdateAsync(promotion, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await promotions.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
