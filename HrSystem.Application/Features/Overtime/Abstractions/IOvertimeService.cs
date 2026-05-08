using HrSystem.Application.Overtime;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Abstractions;

public interface IOvertimeService
{
    Task<OvertimeAutoGenerationResultDto> AutoGenerateFromAttendanceAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        bool createIfMissing = true,
        CancellationToken cancellationToken = default);

    Task ApproveAsync(int overtimeRequestId, string decidedBy, string? note = null, CancellationToken cancellationToken = default);
    Task RejectAsync(int overtimeRequestId, string decidedBy, string? note = null, CancellationToken cancellationToken = default);

    Task<OvertimePolicy> GetActivePolicyAsync(CancellationToken cancellationToken = default);
}
