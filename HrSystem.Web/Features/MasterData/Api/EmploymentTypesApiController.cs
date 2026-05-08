using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employment-types")]
[Authorize]
public sealed class EmploymentTypesApiController(ICrudService<EmploymentType> employmentTypes) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmploymentType>>> List(CancellationToken cancellationToken) =>
        Ok(await employmentTypes.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmploymentType>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await employmentTypes.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmploymentType>> Create(EmploymentType employmentType, CancellationToken cancellationToken)
    {
        var created = await employmentTypes.CreateAsync(employmentType, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmploymentType employmentType, CancellationToken cancellationToken)
    {
        if (id != employmentType.Id)
        {
            return BadRequest();
        }

        await employmentTypes.UpdateAsync(employmentType, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await employmentTypes.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
