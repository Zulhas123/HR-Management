using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Repositories;

public interface ILeaveRequestRepository : IRepository<LeaveRequest>
{
    Task<IReadOnlyList<LeaveRequest>> ListApprovedOverlappingDateRangeAsync(
        DateOnly fromInclusive,
        DateOnly toInclusive,
        CancellationToken cancellationToken = default);
}
