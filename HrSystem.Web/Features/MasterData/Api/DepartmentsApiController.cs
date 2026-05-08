using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/departments")]
[Authorize]
public sealed class DepartmentsApiController(ICrudService<Department> departments) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Department>>> List(CancellationToken cancellationToken) =>
        Ok(await departments.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Department>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await departments.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Department>> Create(Department department, CancellationToken cancellationToken)
    {
        var created = await departments.CreateAsync(department, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Department department, CancellationToken cancellationToken)
    {
        if (id != department.Id)
        {
            return BadRequest();
        }

        await departments.UpdateAsync(department, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await departments.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
