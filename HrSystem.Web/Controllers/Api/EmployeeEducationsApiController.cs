using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-educations")]
[Authorize]
public sealed class EmployeeEducationsApiController(ICrudService<EmployeeEducation> educations) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeEducation>>> List([FromQuery] int? employeeId, CancellationToken cancellationToken)
    {
        var items = await educations.ListAsync(cancellationToken);
        if (employeeId is not null)
        {
            items = items.Where(d => d.EmployeeId == employeeId.Value).ToList();
        }

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeEducation>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await educations.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeEducation>> Create(EmployeeEducation education, CancellationToken cancellationToken)
    {
        var created = await educations.CreateAsync(education, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeEducation education, CancellationToken cancellationToken)
    {
        if (id != education.Id)
        {
            return BadRequest();
        }

        await educations.UpdateAsync(education, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await educations.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

