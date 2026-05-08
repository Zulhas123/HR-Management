using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-documents")]
[Authorize]
public sealed class EmployeeDocumentsApiController(ICrudService<EmployeeDocument> documents) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeDocument>>> List([FromQuery] int? employeeId, CancellationToken cancellationToken)
    {
        var items = await documents.ListAsync(cancellationToken);
        if (employeeId is not null)
        {
            items = items.Where(d => d.EmployeeId == employeeId.Value).ToList();
        }

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDocument>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await documents.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDocument>> Create(EmployeeDocument document, CancellationToken cancellationToken)
    {
        var created = await documents.CreateAsync(document, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeDocument document, CancellationToken cancellationToken)
    {
        if (id != document.Id)
        {
            return BadRequest();
        }

        await documents.UpdateAsync(document, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await documents.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

