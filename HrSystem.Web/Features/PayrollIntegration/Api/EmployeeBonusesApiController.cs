using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-bonuses")]
[Authorize]
public sealed class EmployeeBonusesApiController(ICrudService<EmployeeBonus> bonuses) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeBonus>>> List(CancellationToken cancellationToken) =>
        Ok(await bonuses.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeBonus>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await bonuses.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeBonus>> Create(EmployeeBonus bonus, CancellationToken cancellationToken)
    {
        bonus.Employee = null;
        var created = await bonuses.CreateAsync(bonus, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeBonus bonus, CancellationToken cancellationToken)
    {
        if (id != bonus.Id)
        {
            return BadRequest();
        }

        bonus.Employee = null;
        await bonuses.UpdateAsync(bonus, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await bonuses.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
