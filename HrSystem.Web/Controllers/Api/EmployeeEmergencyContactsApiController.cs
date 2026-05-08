using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-emergency-contacts")]
[Authorize]
public sealed class EmployeeEmergencyContactsApiController(ICrudService<EmployeeEmergencyContact> contacts) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeEmergencyContact>>> List([FromQuery] int? employeeId, CancellationToken cancellationToken)
    {
        var items = await contacts.ListAsync(cancellationToken);
        if (employeeId is not null)
        {
            items = items.Where(d => d.EmployeeId == employeeId.Value).ToList();
        }

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeEmergencyContact>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await contacts.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeEmergencyContact>> Create(EmployeeEmergencyContact contact, CancellationToken cancellationToken)
    {
        var created = await contacts.CreateAsync(contact, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeEmergencyContact contact, CancellationToken cancellationToken)
    {
        if (id != contact.Id)
        {
            return BadRequest();
        }

        await contacts.UpdateAsync(contact, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await contacts.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

