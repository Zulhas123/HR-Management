using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HrSystem.Web.Security;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/employees")]
[Authorize]
public sealed class EmployeesApiController(IEmployeeService employees) : ControllerBase
{
    [HttpGet]
    [Permission("employees.read")]
    public async Task<ActionResult<IReadOnlyList<Employee>>> List(CancellationToken cancellationToken) =>
        Ok(await employees.ListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    [Permission("employees.read")]
    public async Task<ActionResult<Employee>> Get(int id, CancellationToken cancellationToken)
    {
        var entity = await employees.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    [Permission("employees.write")]
    public async Task<ActionResult<Employee>> Create(Employee employee, CancellationToken cancellationToken)
    {
        var created = await employees.CreateAsync(employee, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Permission("employees.write")]
    public async Task<IActionResult> Update(int id, Employee employee, CancellationToken cancellationToken)
    {
        if (id != employee.Id)
        {
            return BadRequest();
        }

        await employees.UpdateAsync(employee, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Permission("employees.write")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await employees.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
