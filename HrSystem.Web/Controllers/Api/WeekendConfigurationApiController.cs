using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/weekends")]
[Authorize]
public sealed class WeekendConfigurationApiController(ICrudService<WeekendConfiguration> weekends) : ControllerBase
{
    [HttpGet("current")]
    public async Task<ActionResult<WeekendConfiguration>> GetCurrent(CancellationToken cancellationToken)
    {
        var current = (await weekends.ListAsync(cancellationToken)).FirstOrDefault();
        return current is null ? NotFound() : Ok(current);
    }

    [HttpPost]
    public async Task<ActionResult<WeekendConfiguration>> Create(WeekendConfiguration config, CancellationToken cancellationToken)
    {
        var created = await weekends.CreateAsync(config, cancellationToken);
        return CreatedAtAction(nameof(GetCurrent), new { }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, WeekendConfiguration config, CancellationToken cancellationToken)
    {
        if (id != config.Id)
        {
            return BadRequest();
        }

        await weekends.UpdateAsync(config, cancellationToken);
        return NoContent();
    }
}

