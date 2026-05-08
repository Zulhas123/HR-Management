using HrSystem.Application.PayrollIntegration;

namespace HrSystem.Application.Abstractions;

public interface IPayrollIntegrationService
{
    Task<PayrollPeriodSummaryDto> GetPeriodSummaryAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default);

    Task<PayrollPeriodExportDto> ExportPeriodAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        bool onlyUnsyncedBonusesAndAdjustments = true,
        bool markBonusesAndAdjustmentsAsSynced = false,
        CancellationToken cancellationToken = default);
}
