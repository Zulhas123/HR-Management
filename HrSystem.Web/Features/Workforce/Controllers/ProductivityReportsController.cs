using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class ProductivityReportsController(
    IWorkforceReportingService reporting,
    ICrudService<Department> departments) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var vm = new ProductivityReportVm();
        await PopulateLookupsAsync(vm, cancellationToken);
        vm.Report = await reporting.GetProductivityReportAsync(
            DateOnly.FromDateTime(vm.FromInclusive),
            DateOnly.FromDateTime(vm.ToInclusive),
            vm.DepartmentId,
            cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ProductivityReportVm vm, CancellationToken cancellationToken)
    {
        await PopulateLookupsAsync(vm, cancellationToken);
        vm.Report = await reporting.GetProductivityReportAsync(
            DateOnly.FromDateTime(vm.FromInclusive),
            DateOnly.FromDateTime(vm.ToInclusive),
            vm.DepartmentId,
            cancellationToken);
        return View(vm);
    }

    private async Task PopulateLookupsAsync(ProductivityReportVm vm, CancellationToken cancellationToken)
    {
        var items = await departments.ListAsync(cancellationToken);
        vm.Departments = new SelectList(items, nameof(Department.Id), nameof(Department.Name), vm.DepartmentId);
    }
}
