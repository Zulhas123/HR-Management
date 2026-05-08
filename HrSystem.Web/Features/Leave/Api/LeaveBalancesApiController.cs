using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

[ApiController]
[Route("api/leave-balances")]
[Authorize]
public sealed class LeaveBalancesApiController(
    ICrudService<LeaveType> leaveTypes,
    ILeaveBalanceService balanceService,
    ILeaveBalanceRepository balances) : ControllerBase
{
    [HttpGet("{employeeId:int}/{year:int}")]
    public async Task<ActionResult<IReadOnlyList<LeaveBalance>>> List(int employeeId, int year, CancellationToken cancellationToken)
    {
        var types = await leaveTypes.ListAsync(cancellationToken);
        foreach (var type in types)
        {
            _ = await balanceService.GetOrCreateAsync(employeeId, type.Id, year, cancellationToken);
        }

        return Ok(await balances.ListByEmployeeYearAsync(employeeId, year, cancellationToken));
    }
}
