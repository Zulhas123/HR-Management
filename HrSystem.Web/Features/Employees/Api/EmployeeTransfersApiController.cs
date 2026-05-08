using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-transfers")]
[Authorize]
public sealed class EmployeeTransfersApiController(ICrudService<EmployeeTransfer> transfers) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeTransfer>>> List([FromQuery] int? employeeId, CancellationToken cancellationToken)
    {
        var items = await transfers.ListAsync(cancellationToken);
        if (employeeId is not null)
        {
            items = items.Where(d => d.EmployeeId == employeeId.Value).ToList();
        }

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeTransfer>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await transfers.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeTransfer>> Create(EmployeeTransfer transfer, CancellationToken cancellationToken)
    {
        var created = await transfers.CreateAsync(transfer, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeTransfer transfer, CancellationToken cancellationToken)
    {
        if (id != transfer.Id)
        {
            return BadRequest();
        }

        await transfers.UpdateAsync(transfer, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await transfers.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
