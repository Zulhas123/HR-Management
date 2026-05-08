using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/daily-work-logs")]
[Authorize]
public sealed class DailyWorkLogsApiController(ICrudService<DailyWorkLog> logs) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DailyWorkLog>>> List(CancellationToken cancellationToken) =>
        Ok(await logs.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DailyWorkLog>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await logs.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<DailyWorkLog>> Create(DailyWorkLog log, CancellationToken cancellationToken)
    {
        log.Employee = null;
        log.EmployeeTask = null;
        var created = await logs.CreateAsync(log, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DailyWorkLog log, CancellationToken cancellationToken)
    {
        if (id != log.Id)
        {
            return BadRequest();
        }

        log.Employee = null;
        log.EmployeeTask = null;
        await logs.UpdateAsync(log, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await logs.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
