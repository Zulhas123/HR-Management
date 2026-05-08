using HrSystem.Application.Workforce;

namespace HrSystem.Application.Abstractions;

public interface IWorkforceReportingService
{
    Task<WorkforceProductivityReportDto> GetProductivityReportAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        int? departmentId = null,
        CancellationToken cancellationToken = default);
}
