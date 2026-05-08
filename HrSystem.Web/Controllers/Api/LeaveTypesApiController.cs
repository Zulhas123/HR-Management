using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/leave-types")]
[Authorize]
public sealed class LeaveTypesApiController(ICrudService<LeaveType> leaveTypes) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LeaveType>>> List(CancellationToken cancellationToken) =>
        Ok(await leaveTypes.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeaveType>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await leaveTypes.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveType>> Create(LeaveType leaveType, CancellationToken cancellationToken)
    {
        var created = await leaveTypes.CreateAsync(leaveType, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LeaveType leaveType, CancellationToken cancellationToken)
    {
        if (id != leaveType.Id)
        {
            return BadRequest();
        }

        await leaveTypes.UpdateAsync(leaveType, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await leaveTypes.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

