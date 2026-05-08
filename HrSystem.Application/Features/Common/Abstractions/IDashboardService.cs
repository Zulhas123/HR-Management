using HrSystem.Application.Features.Common.Models;

namespace HrSystem.Application.Features.Common.Abstractions;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(DateOnly? today = null, CancellationToken cancellationToken = default);
}

