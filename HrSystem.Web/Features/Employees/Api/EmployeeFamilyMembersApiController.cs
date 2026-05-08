using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-family-members")]
[Authorize]
public sealed class EmployeeFamilyMembersApiController(ICrudService<EmployeeFamilyMember> family) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeFamilyMember>>> List([FromQuery] int? employeeId, CancellationToken cancellationToken)
    {
        var items = await family.ListAsync(cancellationToken);
        if (employeeId is not null)
        {
            items = items.Where(d => d.EmployeeId == employeeId.Value).ToList();
        }

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeFamilyMember>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await family.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeFamilyMember>> Create(EmployeeFamilyMember member, CancellationToken cancellationToken)
    {
        var created = await family.CreateAsync(member, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeFamilyMember member, CancellationToken cancellationToken)
    {
        if (id != member.Id)
        {
            return BadRequest();
        }

        await family.UpdateAsync(member, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await family.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
