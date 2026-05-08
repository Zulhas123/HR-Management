using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/attendance")]
[Authorize]
public sealed class AttendanceApiController(ICrudService<AttendanceRecord> attendance) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AttendanceRecord>>> List(CancellationToken cancellationToken) =>
        Ok(await attendance.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AttendanceRecord>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await attendance.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceRecord>> Create(AttendanceRecord record, CancellationToken cancellationToken)
    {
        var created = await attendance.CreateAsync(record, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, AttendanceRecord record, CancellationToken cancellationToken)
    {
        if (id != record.Id)
        {
            return BadRequest();
        }

        await attendance.UpdateAsync(record, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await attendance.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

