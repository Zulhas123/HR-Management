using HrSystem.Application.Abstractions;
using HrSystem.Application.PayrollIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers.Api;

public sealed class PayrollExportRequest
{
    public DateOnly FromInclusive { get; set; }
    public DateOnly ToInclusive { get; set; }
    public bool OnlyUnsyncedBonusesAndAdjustments { get; set; } = true;
    public bool MarkBonusesAndAdjustmentsAsSynced { get; set; } = false;
}

[ApiController]
[Route("api/payroll-integration")]
[Authorize]
public sealed class PayrollIntegrationApiController(IPayrollIntegrationService payroll) : ControllerBase
{
    [HttpGet("period-summary")]
    public async Task<ActionResult<PayrollPeriodSummaryDto>> PeriodSummary(
        [FromQuery] DateOnly fromInclusive,
        [FromQuery] DateOnly toInclusive,
        CancellationToken cancellationToken)
    {
        var result = await payroll.GetPeriodSummaryAsync(fromInclusive, toInclusive, cancellationToken);
        return Ok(result);
    }

    [HttpPost("export")]
    public async Task<ActionResult<PayrollPeriodExportDto>> Export(
        PayrollExportRequest request,
        CancellationToken cancellationToken)
    {
        var result = await payroll.ExportPeriodAsync(
            request.FromInclusive,
            request.ToInclusive,
            request.OnlyUnsyncedBonusesAndAdjustments,
            request.MarkBonusesAndAdjustmentsAsSynced,
            cancellationToken);

        return Ok(result);
    }
}
