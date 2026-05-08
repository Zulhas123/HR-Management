using HrSystem.Application.Workforce;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.ViewModels;

public sealed class ProductivityReportVm
{
    public DateTime FromInclusive { get; set; } = DateTime.Today.AddDays(-6);
    public DateTime ToInclusive { get; set; } = DateTime.Today;
    public int? DepartmentId { get; set; }

    public SelectList? Departments { get; set; }

    public WorkforceProductivityReportDto? Report { get; set; }
}
