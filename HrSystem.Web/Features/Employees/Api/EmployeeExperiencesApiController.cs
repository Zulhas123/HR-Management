using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-experiences")]
[Authorize]
public sealed class EmployeeExperiencesApiController(ICrudService<EmployeeExperience> experiences) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeExperience>>> List([FromQuery] int? employeeId, CancellationToken cancellationToken)
    {
        var items = await experiences.ListAsync(cancellationToken);
        if (employeeId is not null)
        {
            items = items.Where(d => d.EmployeeId == employeeId.Value).ToList();
        }

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeExperience>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await experiences.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeExperience>> Create(EmployeeExperience experience, CancellationToken cancellationToken)
    {
        var created = await experiences.CreateAsync(experience, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeExperience experience, CancellationToken cancellationToken)
    {
        if (id != experience.Id)
        {
            return BadRequest();
        }

        await experiences.UpdateAsync(experience, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await experiences.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
