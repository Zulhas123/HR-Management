using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class LeaveBalancesController(
    ICrudService<Employee> employees,
    ICrudService<LeaveType> leaveTypes,
    ILeaveBalanceService balanceService,
    ILeaveBalanceRepository balances) : Controller
{
    public async Task<IActionResult> Index(int? employeeId, int? year, CancellationToken cancellationToken)
    {
        var vm = new LeaveBalanceIndexVm
        {
            EmployeeId = employeeId,
            Year = year ?? DateTime.Today.Year
        };

        vm.Employees = (await employees.ListAsync(cancellationToken))
            .Select(e => new SelectListItem($"{e.EmployeeCode} - {e.FirstName} {e.LastName}", e.Id.ToString(), employeeId == e.Id))
            .ToList();

        if (employeeId.HasValue)
        {
            var types = await leaveTypes.ListAsync(cancellationToken);
            foreach (var type in types)
            {
                _ = await balanceService.GetOrCreateAsync(employeeId.Value, type.Id, vm.Year, cancellationToken);
            }

            vm.Balances = await balances.ListByEmployeeYearAsync(employeeId.Value, vm.Year, cancellationToken);
        }

        return View(vm);
    }
}
