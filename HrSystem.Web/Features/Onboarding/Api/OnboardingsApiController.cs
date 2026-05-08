using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/onboardings")]
[Authorize]
public sealed class OnboardingsApiController(ICrudService<EmployeeOnboarding> onboardings) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeOnboarding>>> List(CancellationToken cancellationToken) =>
        Ok(await onboardings.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeOnboarding>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await onboardings.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeOnboarding>> Create(EmployeeOnboarding onboarding, CancellationToken cancellationToken)
    {
        var created = await onboardings.CreateAsync(onboarding, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeOnboarding onboarding, CancellationToken cancellationToken)
    {
        if (id != onboarding.Id)
        {
            return BadRequest();
        }

        await onboardings.UpdateAsync(onboarding, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await onboardings.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
