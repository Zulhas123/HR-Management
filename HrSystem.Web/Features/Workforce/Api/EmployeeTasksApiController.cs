using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-tasks")]
[Authorize]
public sealed class EmployeeTasksApiController(ICrudService<EmployeeTask> tasks) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeTask>>> List(CancellationToken cancellationToken) =>
        Ok(await tasks.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeTask>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await tasks.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeTask>> Create(EmployeeTask task, CancellationToken cancellationToken)
    {
        task.Employee = null;
        var created = await tasks.CreateAsync(task, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeTask task, CancellationToken cancellationToken)
    {
        if (id != task.Id)
        {
            return BadRequest();
        }

        task.Employee = null;
        await tasks.UpdateAsync(task, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await tasks.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
