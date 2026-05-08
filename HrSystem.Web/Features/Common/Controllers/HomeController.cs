using System.Diagnostics;
using HrSystem.Application.Abstractions;
using HrSystem.Application.Features.Common.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HrSystem.Web.Models;
using HrSystem.Web.ViewModels;
using Microsoft.Extensions.Options;

namespace HrSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashboardService _dashboard;
    private readonly ICrudService<OvertimePolicy> _overtimePolicies;
    private readonly ICrudService<WeekendConfiguration> _weekends;
    private readonly DashboardOptions _options;

    public HomeController(
        ILogger<HomeController> logger,
        IDashboardService dashboard,
        ICrudService<OvertimePolicy> overtimePolicies,
        ICrudService<WeekendConfiguration> weekends,
        IOptions<DashboardOptions> options)
    {
        _logger = logger;
        _dashboard = dashboard;
        _overtimePolicies = overtimePolicies;
        _weekends = weekends;
        _options = options.Value;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var summary = await _dashboard.GetSummaryAsync(cancellationToken: cancellationToken);

        var overtimePolicy = (await _overtimePolicies.ListAsync(cancellationToken)).FirstOrDefault();
        var weekendConfig = (await _weekends.ListAsync(cancellationToken)).FirstOrDefault();

        var vm = new DashboardVm
        {
            Summary = summary,
            OvertimePolicy = overtimePolicy is null
                ? null
                : new OvertimePolicyVm(
                    EffectiveFrom: overtimePolicy.EffectiveFrom,
                    NormalMultiplier: overtimePolicy.NormalMultiplier,
                    HolidayMultiplier: overtimePolicy.HolidayMultiplier,
                    ApprovalLevelsRequired: overtimePolicy.ApprovalLevelsRequired),
            WeekendConfig = weekendConfig is null
                ? null
                : new WeekendConfigVm(
                    Friday: weekendConfig.Friday,
                    Saturday: weekendConfig.Saturday,
                    Sunday: weekendConfig.Sunday,
                    Monday: weekendConfig.Monday,
                    Tuesday: weekendConfig.Tuesday,
                    Wednesday: weekendConfig.Wednesday,
                    Thursday: weekendConfig.Thursday),
            ImportantPolicies = _options.ImportantPolicies
                .Select(x => new ImportantPolicyVm(x.Title, x.Body, x.Url))
                .ToList(),
            RefreshSeconds = Math.Clamp(_options.RefreshSeconds, 5, 300),
        };

        return View(vm);
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
