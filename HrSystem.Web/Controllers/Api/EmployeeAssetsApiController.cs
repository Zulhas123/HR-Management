using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employee-assets")]
[Authorize]
public sealed class EmployeeAssetsApiController(ICrudService<EmployeeAssetAssignment> assets) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeAssetAssignment>>> List(CancellationToken cancellationToken) =>
        Ok(await assets.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeAssetAssignment>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await assets.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeAssetAssignment>> Create(EmployeeAssetAssignment asset, CancellationToken cancellationToken)
    {
        var created = await assets.CreateAsync(asset, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeAssetAssignment asset, CancellationToken cancellationToken)
    {
        if (id != asset.Id)
        {
            return BadRequest();
        }

        await assets.UpdateAsync(asset, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await assets.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}

