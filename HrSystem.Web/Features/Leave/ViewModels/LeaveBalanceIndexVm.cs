using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.ViewModels;

public sealed class LeaveBalanceIndexVm
{
    public int? EmployeeId { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;

    public List<SelectListItem> Employees { get; set; } = [];

    public IReadOnlyList<HrSystem.Domain.Entities.LeaveBalance> Balances { get; set; } = [];
}
